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
        public async Task AddNewFileItem(AddFileSystemItemRequest fileSystemRequest)
        {


            bool nameAvailable = await IsNameAvailable(fileSystemRequest);
            if (!nameAvailable)
            {
                throw new FileSystemNameException("Folder with same name already exists");
            }
            FileSystemItem? parent;

            parent = await getFileItemWithPath(fileSystemRequest.Path);
            Guid currentUserId = await getCurrentUserId();

            var newFileItem = new FileSystemItem(parent, FileItemType.FOLDER, fileSystemRequest.Name, currentUserId);
            await directoryRepository.AddNewFileSystemItem(newFileItem);

        }



        public async Task<bool> IsNameAvailable(AddFileSystemItemRequest fileSystemRequest)
        {

            FileSystemItem? parent = await getFileItemWithPath(fileSystemRequest.Path);
            Guid currentUserId=await getCurrentUserId();
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
            Guid currentUserId= await getCurrentUserId();
            List<FileSystemItem> childList= await directoryRepository.GetAllChildrensInFolder(parent,currentUserId);
            
            return childList;

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
            FileSystemItem? parent = null;
            FileSystemItem? resultfileSystemItem = null;
            foreach (var pathSegement in pathSegements)
            {
                resultfileSystemItem = await tryToGetFileSystemItemForSegment(parent, pathSegement);
            }
            return resultfileSystemItem;
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
            Guid currentUserId = await getCurrentUserId();
            var fileSystemItem = await directoryRepository.GetFileItemFromParentDirectory(parent, pathSegement,currentUserId);

            if (fileSystemItem == null)
            {
                throw new Exception("Path not found");
            }
            return fileSystemItem;
        }

        private async Task<Guid> getCurrentUserId()
        {
            ApplicationUser currentUser = await _userService.GetCurrentUser();
            Guid currentUserId = currentUser.Id;
            return currentUserId;
        }
    }
}
