using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.Domain.Enums;
using IdeaFrame.Core.Domain.Exceptions;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Services
{
    public class DirectoryService : IDirectoryService
    {
        IDirectoryRepository directoryRepository;
        IUserService _userService;

        public DirectoryService(IDirectoryRepository directoryRepository,IUserService userService)
        {
            this.directoryRepository = directoryRepository;
            _userService = userService;

        }
        public async Task AddNewFileItem(FileSystemItemDTO fileSystemRequest)
        {


            bool nameAvailable = await IsNameAvailable(fileSystemRequest);
            if (!nameAvailable)
            {
                throw new FileSystemNameException("Folder with same name already exists");
            }
            FileSystemItem? parent;

            parent = await getFolderItemWithPath(fileSystemRequest.Path);
            Guid currentUserId = await _userService.GetCurrentUserId();

            var newFileItem = new FileSystemItem(parent, fileSystemRequest.Type, fileSystemRequest.Name, currentUserId);
            await directoryRepository.AddNewFileSystemItem(newFileItem);

        }

        public async Task MoveFileItem(MoveFileTimeRequestDTO fileToMove)
        {
            
            FileSystemItem? fileItemToMove = await GetFileItem(fileToMove);
            FileSystemItem? newParent = await this.getFolderItemWithPath(fileToMove.NewPath);
            await validationForMoveFileItem(fileToMove, fileItemToMove, newParent);


            await this.directoryRepository.MoveFileSystemItem(fileItemToMove, newParent);
            return;
        }



        public async Task<bool> IsNameAvailable(FileSystemItemDTO fileSystemRequest)
        {

            return await isNameAvailable(fileSystemRequest,fileSystemRequest.Name);
        }

        public async Task<bool> IsNameAvailable(MoveFileTimeRequestDTO fileSystemRequest)
        {
            var checkIfNameIsAvailableDTO = fileSystemRequest.GetFileSystemItemDtoWhereNewPathIsSetAsPath();
            return await isNameAvailable(checkIfNameIsAvailableDTO, fileSystemRequest.Name);
        }



        public async Task<List<FileSystemItem>> GetAllChildrensInPath(String path)
        {
            FileSystemItem? parent = await getFolderItemWithPath(path);
            Guid currentUserId= await _userService.GetCurrentUserId();
            List<FileSystemItem> childList= await directoryRepository.GetAllChildrensInFolder(parent,currentUserId);
            
            return childList;

        }


        public async Task RemoveFileItem(FileSystemItemDTO fileToRemoveDTO)
        {
            FileSystemItem? fileItemToRemove = await GetFileItem(fileToRemoveDTO);
            if (fileItemToRemove.Type == FileItemType.FILE)
            {
                await this.directoryRepository.RemoveFileSystemItem(fileItemToRemove);
                return;
            }

            await removeAllDescendantsOfFolder(fileItemToRemove);
            await this.directoryRepository.RemoveFileSystemItem(fileItemToRemove);


        }

        public async Task EditFileItemName(EditFileItemNameDTO editFileItemDTO)
        {
            FileSystemItem? fileItemToEdit = await GetFileItem(editFileItemDTO);
            bool nameIsAvailable=await this.isNameAvailable(editFileItemDTO, editFileItemDTO.NewName);
            if(!nameIsAvailable)
                throw new FileSystemNameException("File item with same name already exists in folder");

            await this.directoryRepository.RenameFileSystemItem(fileItemToEdit, editFileItemDTO.NewName);
        }

        public async Task<FileSystemItem?> GetFileItem(FileSystemItemDTO fileItemDTO)
        {
            var parent = await this.getFolderItemWithPath(fileItemDTO.Path);
            Guid currentUserId = await _userService.GetCurrentUserId();
            FileSystemItemSearchInDbDTO fileItemToGetDTO = new FileSystemItemSearchInDbDTO(fileItemDTO.Name, parent, fileItemDTO.Type, currentUserId);
            var result = await this.directoryRepository.GetFileItemFromParentDirectory(fileItemToGetDTO);
            return result;
        }

        private async Task<bool> isNameAvailable(FileSystemItemDTO fileSystemRequest, string nameToCheck)
        {
            FileSystemItem? parent = await getFolderItemWithPath(fileSystemRequest.Path);
            Guid currentUserId = await _userService.GetCurrentUserId();
            List<FileSystemItem> fileItemsInParent = await directoryRepository.GetAllChildrensInFolder(parent, currentUserId);
            List<FileSystemItem> fileItemsWithType = getFileItemsWithType(fileSystemRequest.Type, fileItemsInParent);
            foreach (var fileItem in fileItemsWithType)
            {
                if (fileItem.Name == nameToCheck)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task removeAllDescendantsOfFolder(FileSystemItem? fileItemToRemove)
        {
            List<FileSystemItem> fileItemsToRemove = await this.getAllFolderDescendants(fileItemToRemove);
            fileItemsToRemove.Reverse();
            foreach (var elementToRemove in fileItemsToRemove)
            {
                await this.directoryRepository.RemoveFileSystemItem(elementToRemove);

            }
        }

        private async Task<List<FileSystemItem>> getAllFolderDescendants(FileSystemItem fileItem)
        {
            var userId= await _userService.GetCurrentUserId();
            List<FileSystemItem> childrens = await this.directoryRepository.GetAllChildrensInFolder(fileItem,userId);
            List<FileSystemItem> result = new List<FileSystemItem>();
            result.AddRange(childrens);
            foreach (var child in childrens)
            {
                var childDescendants=await getAllFolderDescendants(child);
                result.AddRange(childDescendants);
            }

            return result;
        }



        private static List<FileSystemItem> getFileItemsWithType(FileItemType type, List<FileSystemItem> fileItemsInParent)
        {
            return fileItemsInParent.Where(fileItem => fileItem.Type == type).ToList();
        }

        private async Task validationForMoveFileItem(MoveFileTimeRequestDTO fileToMove, FileSystemItem? fileItemToMove, FileSystemItem? newParent)
        {
            var nameAvailable = await this.IsNameAvailable(fileToMove);
            if (!nameAvailable)
            {
                throw new Exception("Name of file isn't available in new folder");
            }
            if (newParent != null && fileItemToMove.Id == newParent.Id)
            {
                throw new Exception("Cannot move file to itself");
            }
        }



        private async Task<FileSystemItem?> getFolderItemWithPath(string pathStr)
        {
            if(pathStr == "/")
            {
                return null;
            }
            string[] pathSegements = pathStr.Split("/");
            pathSegements = pathSegements.Skip(1).ToArray();
            FileSystemItem? parent = null;

            foreach (var pathSegement in pathSegements)
            {
                parent = await tryToGetFileSystemItemForSegment(parent, pathSegement,FileItemType.FOLDER);
            }
            return parent;
        }

  

        private static bool isChildWithNameAmongChildrens(string newFolderName, List<FileSystemItem> childrens)
        {
            foreach (var child in childrens)
            {
                if (child.Name == newFolderName)
                {
                    return true;
                }
            }
            return false;
        }


        private async Task<FileSystemItem> tryToGetFileSystemItemForSegment(FileSystemItem? parent, string pathSegement, FileItemType fileItemType)
        {
            Guid currentUserId = await _userService.GetCurrentUserId();
            FileSystemItemSearchInDbDTO fileSystemItemToFind = new FileSystemItemSearchInDbDTO(pathSegement,parent,fileItemType,currentUserId);
            var fileSystemItem = await directoryRepository.GetFileItemFromParentDirectory(fileSystemItemToFind);

            if (fileSystemItem == null)
            {
                throw new Exception("Path not found");
            }
            return fileSystemItem;
        }


    }
}
