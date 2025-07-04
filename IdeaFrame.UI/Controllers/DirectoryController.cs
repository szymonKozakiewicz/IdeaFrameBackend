﻿using IdeaFrame.Core.Domain.Entities;
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
        public async Task<IActionResult> AddNewFileItem([FromBody]FileSystemItemDTO newFileItemRequest)
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

        [HttpPost("isFileItemNameAvailable")]
        public async Task<IActionResult> IsFileItemNameAvailable(FileSystemItemDTO fileSystemRequest)
        {
            
            bool isNameAvailable = await this.directoryService.IsNameAvailable(fileSystemRequest);
            if (isNameAvailable)
                return Ok(true);
            else
                return Ok(false);
        }

        [HttpGet("getAllFoldersFromPath")]
        public async Task<IActionResult> GetAllFoldersFromPath(String path)
        {
            List<FileSystemItem> result;
            try
            {
                result = await this.directoryService.GetAllChildrensInPath(path);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(result);
        }

        [HttpPost("removeFileItem")]
        public async Task<IActionResult> RemoveFileItem(FileSystemItemDTO fileItemToRemove)
        {
            try
            {
                await this.directoryService.RemoveFileItem(fileItemToRemove);
            }
            catch(Exception e) {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();

        }

        [HttpPost("moveFileItem")]
        public async Task<IActionResult> MoveFileItem(MoveFileTimeRequestDTO fileItemToMove)
        {
            try
            {
                
                await this.directoryService.MoveFileItem(fileItemToMove);
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();

        }



        [HttpPost("editFileItemName")]
        public async Task<IActionResult> EditFileItemName(EditFileItemNameDTO editFileItem)
        {
            try
            {
                await this.directoryService.EditFileItemName(editFileItem);
            }
            catch (Exception e)
            {

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();

        }
    }
}
