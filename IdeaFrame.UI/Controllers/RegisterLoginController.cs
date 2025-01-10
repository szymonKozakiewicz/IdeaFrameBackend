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
            
            return await tryCreateJwtResponse(loginDTO);

        }

        private async Task<IActionResult> tryCreateJwtResponse(RegisterLoginDTO loginDTO)
        {
            try
            {

                JwtResponse jwtResponse = await this.jwtService.CreateJwtResponse(loginDTO.Login);
                await addRefreshTokenToCookies(loginDTO.Login);
                return Ok(jwtResponse);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private async Task addRefreshTokenToCookies(String login)
        {
            RefreshTokenDto refreshToken = await this.jwtService.CreateRefreshToken(login);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.Expiration
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }
    }
}
