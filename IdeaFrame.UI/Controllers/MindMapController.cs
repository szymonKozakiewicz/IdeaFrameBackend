using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace IdeaFrame.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MindMapController : ControllerBase
    {
        public IMindMapService _mindMapService;
        public MindMapController(IMindMapService mindMapService)
        {
            this._mindMapService = mindMapService;
        }


        [HttpPost("saveMindMap")]
        public async Task<IActionResult> SaveMindMap([FromBody] SaveMindMapDTO saveMindMapDTO)
        {
            List<MindMapNodeDTO> currentNodesList;
            try
            {
                await this._mindMapService.SaveMindMap(saveMindMapDTO);
                currentNodesList = await this._mindMapService.GetMindMap(saveMindMapDTO.FileItem);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving the mind map.");
            }

            return Ok(currentNodesList);
            
        }


        [HttpPost("loadMindMap")]
        public async Task<IActionResult> LoadMindMap([FromBody] FileSystemItemDTO fileSystemItem)
        {
            List<MindMapNodeDTO> nodes;
            try
            {
                nodes = await this._mindMapService.GetMindMap(fileSystemItem);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving the mind map.");
            }

            return Ok(nodes);

        }
    }
}
