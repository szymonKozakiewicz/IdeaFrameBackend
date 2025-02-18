using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdeaFrame.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPanelController : ControllerBase
    {

        IConfiguration _configuration;
        public UserPanelController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("")]
        public async Task<IActionResult> AuthorizedRequest()
        {
            Console.WriteLine("Authorized request");
            string userName = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Ok();
        }


    }
}
