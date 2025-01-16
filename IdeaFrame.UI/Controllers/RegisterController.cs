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
    public class RegisterController : ControllerBase
    {
        IUserService userService;

        public RegisterController(IUserService userService)
        {
            this.userService = userService;

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


    }
}
