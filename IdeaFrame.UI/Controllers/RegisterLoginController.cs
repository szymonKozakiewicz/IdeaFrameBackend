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
        public RegisterLoginController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpPost("registerNewUser")]
       public async Task<IActionResult> RegisterNewUser([FromBody]RegisterLoginDTO newUser)
       {

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
