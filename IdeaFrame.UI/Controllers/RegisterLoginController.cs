using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace IdeaFrame.UI.Controllers
{
    [Route("api/[controller]")]
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
            JwtResponse jwtResponse;
            return await tryCreateJwtResponse(loginDTO, out jwtResponse);

        }

        private async Task<IActionResult> tryCreateJwtResponse(RegisterLoginDTO loginDTO, out JwtResponse jwtResponse)
        {
            try
            {

                jwtResponse = await this.jwtService.CreateJwtResponse(loginDTO.Login);
                return Ok(jwtResponse);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
