using IdeaFrame.Core.Domain.Entities;
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

        public Task<FileSystemItem?> GetFileItemFromParentDirectory(FileSystemItem parent, String fileItemName, Guid ownerId);

        public Task<List<FileSystemItem>> GetAllChildrensInFolder(FileSystemItem parent,Guid ownerId);

        public Task AddNewFileSystemItem(FileSystemItem newFolder);
    }
}
