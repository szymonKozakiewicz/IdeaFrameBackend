using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Core.ServiceContracts;
using IdeaFrame.Infrastructure.DbContextCustom;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Infrastructure.Repositories
{
    public class DirectoryRepository : IDirectoryRepository
    {

        private MyDbContexSqlServer _dbContext;

        public DirectoryRepository(MyDbContexSqlServer _context)
        {
            this._dbContext = _context;

        }

        public async Task AddNewFileSystemItem(FileSystemItem newFolder)
        {
           
           await this._dbContext.AddAsync(newFolder);
           await this._dbContext.SaveChangesAsync();
        }

        public async Task<List<FileSystemItem>> GetAllChildrensInFolder(FileSystemItem parent, Guid ownerId)
        {
            bool parentIsRootFolder = parent == null;
  
            if (parentIsRootFolder)
            {
                return await GetAllChildrensInFolderForRootParent(ownerId);
            }
            List<FileSystemItem>result=await _dbContext.FileSystemItems
                .Where(x=>x.ParentId==parent.Id)
                .ToListAsync();
            return result;
        }



        public async Task<FileSystemItem?> GetFileItemFromParentDirectory(FileSystemItem? parent, string fileItemName, Guid ownerId)
        {
            bool parentIsRootFolder = parent == null;
            if (parentIsRootFolder)
            {
                return await getFileItemFromRootDirectory(fileItemName,ownerId);
            }
            FileSystemItem? result = await _dbContext.FileSystemItems
                .Where(x => x.ParentId == parent.Id && x.Name == fileItemName)
                .FirstOrDefaultAsync();
            return result;

        }

        private async Task<List<FileSystemItem>> GetAllChildrensInFolderForRootParent(Guid ownerId)
        {
            return await _dbContext.FileSystemItems
                .Where(x => x.ParentId == null && x.OwnerId==ownerId)
                .ToListAsync();
        }



        private async Task<FileSystemItem?> getFileItemFromRootDirectory(string fileItemName,Guid ownerId)
        {
            return await _dbContext.FileSystemItems
                .Where(x => x.ParentId == null && x.Name == fileItemName && x.OwnerId==ownerId)
                .FirstOrDefaultAsync();
        }
    }
}
