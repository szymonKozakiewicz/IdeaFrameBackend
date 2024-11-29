using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
       public async Task<IActionResult> registerNewUser([FromBody]RegisterLoginDTO newUser)
       {
            var result=await userService.AddNewUser(newUser);
            if (result.Succeeded)
                return Ok();
            else return BadRequest();
       }
    }
}
