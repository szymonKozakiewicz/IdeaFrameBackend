using IdeaFrame.Core.Domain.Exceptions;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Enumeration;

namespace IdeaFrame.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        IDirectoryService directoryService;
        public DirectoryController(IDirectoryService directoryService)
        {
            this.directoryService = directoryService;
        }

        [HttpPost("addNewFileItem")]
        public async Task<IActionResult> AddNewFileItem([FromBody]AddFileSystemItemRequest newFileItemRequest)
        {
            try
            {

                await this.directoryService.AddNewFileItem(newFileItemRequest);
    
            }
            catch(FileSystemNameException e)
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();

        }
    }
}
