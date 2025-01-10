using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace IdeaFrame.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPanelController : Controller
    {

        IConfiguration _configuration;
        public UserPanelController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("authorizedRequest")]
        public async Task<IActionResult> AuthorizedRequest()
        {
            Console.WriteLine("Authorized request");

            return Ok();
        }
    }
}
