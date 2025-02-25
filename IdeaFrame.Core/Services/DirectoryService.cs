using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.Enums;
using IdeaFrame.Core.Domain.Exceptions;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Core.DTO;
using IdeaFrame.Core.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Services
{
    public class DirectoryService : IDirectoryService
    {
        IDirectoryRepository directoryRepository;
        public DirectoryService(IDirectoryRepository directoryRepository)
        {
            this.directoryRepository = directoryRepository;
        }
        public async Task AddNewFileItem(AddFileSystemItemRequest fileSystemRequest)
        {

            FileSystemItem? parent;
            
            parent=await getFileItemWithPath(fileSystemRequest.Path);
            
            if(await this.isFolderWithSameNameInParent(parent,fileSystemRequest.Name))
            {
                throw new FileSystemNameException("Folder with same name already exists");
            }


            var newFileItem=new FileSystemItem(parent, FileItemType.FOLDER, fileSystemRequest.Name);
            await directoryRepository.AddNewFileSystemItem(newFileItem);

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

        private async Task<bool> isFolderWithSameNameInParent(FileSystemItem parent, string newFolderName)
        {
            List<FileSystemItem> childrens = await directoryRepository.GetAllChildrensInFolder(parent);
            return isChildWithNameAmongChildrens(newFolderName, childrens);
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
            var fileSystemItem = await directoryRepository.GetFileItemFromParentDirectory(parent, pathSegement);

            if (fileSystemItem == null)
            {
                throw new Exception("Path not found");
            }
            return fileSystemItem;
        }
    }
}
