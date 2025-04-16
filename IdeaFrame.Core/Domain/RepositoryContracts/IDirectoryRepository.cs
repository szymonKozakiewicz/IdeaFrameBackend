using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Domain.RepositoryContracts
{
    public interface IDirectoryRepository
    {

        public Task<FileSystemItem?> GetFileItemFromParentDirectory(FileSystemItemSearchInDbDTO fileSystemItemToFind);

        public Task<List<FileSystemItem>> GetAllChildrensInFolder(FileSystemItem parent,Guid ownerId);

        public Task AddNewFileSystemItem(FileSystemItem newFolder);

        public Task RemoveFileSystemItem(FileSystemItem fileToRemove);
        public Task MoveFileSystemItem(FileSystemItem fileToMove, FileSystemItem newParent);

        public Task RenameFileSystemItem(FileSystemItem fileToEdit, String fileName);
    }
}
