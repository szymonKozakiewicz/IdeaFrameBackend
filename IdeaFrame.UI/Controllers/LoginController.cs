using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdeaFrame.UI.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class LoginController : ControllerBase
    {

        IUserService userService;
        IJwtService jwtService;
        public LoginController(IUserService userService, IJwtService jwtService)
        {
            this.userService = userService;
            this.jwtService = jwtService;
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
                response = StatusCode(StatusCodes.Status500InternalServerError);
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
