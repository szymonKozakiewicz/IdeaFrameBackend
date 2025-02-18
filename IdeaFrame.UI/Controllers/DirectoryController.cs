using IdeaFrame.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdeaFrame.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        IDirectoryService directoryService;

        [Route("addNewFolder")]
        public async Task<IActionResult> AddNewFolder(String newFolderName, String path)
        {
            try
            {
                await this.directoryService.AddNewFolder(newFolderName, path);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();

        }
    }
}
