using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;


namespace IdeaFrame.UI.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class RegisterLoginController : ControllerBase
    {
        IUserService userService;
        IJwtService jwtService;
        public RegisterLoginController(IUserService userService,IJwtService jwtService)
        {
            this.userService = userService;
            this.jwtService = jwtService;
        }
        [HttpPost("registerNewUser")]
       public async Task<IActionResult> RegisterNewUser([FromBody]RegisterLoginDTO newUser)
       {
            bool loginNotAvailable = !(await userService.IsLoginAvailable(newUser.Login));
            if (loginNotAvailable)
            {
                return BadRequest();
            }


            newUser.Password = newUser.Password + "#";
            var result=await userService.AddNewUser(newUser);
            if (result.Succeeded)
                return Ok();
            else return BadRequest();
       }
        [HttpGet("isLoginAvailable")]
        public async Task<IActionResult> IsLoginAvailable([FromQuery]String login)
        {
     
            bool isLoginAvailable =await this.userService.IsLoginAvailable(login);
            if (isLoginAvailable)
                return Ok(true);
            else 
                return Ok(false);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] RegisterLoginDTO loginDTO)
        {
            bool loginDataCorrect = await this.userService.AreLoginDataCorrect(loginDTO);
            if (!loginDataCorrect)
                return Unauthorized();


            JwtResponse jwtResponse = this.jwtService.CreateJwtResponse(loginDTO.Login);
            IActionResult response;
            
            try
            {
                await addRefreshTokenToCookies(loginDTO.Login);
                response = Ok(jwtResponse);
            }
            catch (Exception e)
            {
                response= StatusCode(StatusCodes.Status500InternalServerError);
            }

            return response;



        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            String refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken == null)
                return Unauthorized();


            JwtResponse jwtResponse = await jwtService.CreateJwtResponseIfTokenValid(refreshToken);

            if (jwtResponse == null)
                return Unauthorized();

            return Ok(jwtResponse);

        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            removeRefreshTokenFromCookies("refreshToken");
            return Ok();

        }


        private async Task addRefreshTokenToCookies(String login)
        {
            var cookieName = "refreshToken";
            removeRefreshTokenFromCookies(cookieName);
            RefreshTokenDto refreshToken = await this.jwtService.CreateRefreshToken(login);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.Expiration,
                Path = "/"
            };
            Response.Cookies.Append(cookieName, refreshToken.Token, cookieOptions);
        }

        private void removeRefreshTokenFromCookies(string cookieName)
        {
            
            Response.Cookies.Delete(cookieName);
            
        }
    }
}
