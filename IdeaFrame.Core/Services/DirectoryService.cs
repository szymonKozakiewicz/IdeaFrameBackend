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

            parent = await getFileItemWithPath(fileSystemRequest.Path);
            Guid currentUserId = await _userService.GetCurrentUserId();

            var newFileItem = new FileSystemItem(parent, FileItemType.FOLDER, fileSystemRequest.Name, currentUserId);
            await directoryRepository.AddNewFileSystemItem(newFileItem);

        }

        public async Task MoveFileItem(MoveFileTimeRequestDTO fileToMove)
        {
            FileSystemItem? fileItemToMove = await getFileItem(fileToMove);
            FileSystemItem? newParent=await this.getFileItemWithPath(fileToMove.NewPath);
            await this.directoryRepository.MoveFileSystemItem(fileItemToMove, newParent);
            return;
        }



        public async Task<bool> IsNameAvailable(FileSystemItemDTO fileSystemRequest)
        {

            FileSystemItem? parent = await getFileItemWithPath(fileSystemRequest.Path);
            Guid currentUserId= await _userService.GetCurrentUserId();
            List<FileSystemItem> fileItemsInParent = await directoryRepository.GetAllChildrensInFolder(parent,currentUserId);
            List<FileSystemItem> fileItemsWithType = getFileItemsWithType(fileSystemRequest.Type, fileItemsInParent);
            foreach (var fileItem in fileItemsWithType)
            {
                if (fileItem.Name == fileSystemRequest.Name)
                {
                    return false;
                }
            }
            return true;
        }


        public async Task<List<FileSystemItem>> GetAllChildrensInPath(String path)
        {
            FileSystemItem? parent = await getFileItemWithPath(path);
            Guid currentUserId= await _userService.GetCurrentUserId();
            List<FileSystemItem> childList= await directoryRepository.GetAllChildrensInFolder(parent,currentUserId);
            
            return childList;

        }


        public async Task RemoveFileItem(FileSystemItemDTO fileToRemoveDTO)
        {
            FileSystemItem? fileItemToRemove = await getFileItem(fileToRemoveDTO);
            if (fileItemToRemove.Type == FileItemType.FILE)
            {
                await this.directoryRepository.RemoveFileSystemItem(fileItemToRemove);
                return;
            }

            await removeAllDescendantsOfFolder(fileItemToRemove);
            await this.directoryRepository.RemoveFileSystemItem(fileItemToRemove);


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

        private async Task<FileSystemItem?> getFileItem(FileSystemItemDTO fileItemDTO)
        {
            var fullPath = fileItemDTO.GetPathPlusFileItemName();
            var result = await this.getFileItemWithPath(fullPath);
            return result;
        }

        private static List<FileSystemItem> getFileItemsWithType(FileItemType type, List<FileSystemItem> fileItemsInParent)
        {
            return fileItemsInParent.Where(fileItem => fileItem.Type == type).ToList();
        }

        private async Task<FileSystemItem?> getFileItemWithPath(string pathStr)
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
                parent = await tryToGetFileSystemItemForSegment(parent, pathSegement);
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


        private async Task<FileSystemItem> tryToGetFileSystemItemForSegment(FileSystemItem? parent, string pathSegement)
        {
            Guid currentUserId = await _userService.GetCurrentUserId();
            var fileSystemItem = await directoryRepository.GetFileItemFromParentDirectory(parent, pathSegement,currentUserId);

            if (fileSystemItem == null)
            {
                throw new Exception("Path not found");
            }
            return fileSystemItem;
        }


    }
}
