using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.Enums;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Core.DTO;
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



        public async Task<FileSystemItem?> GetFileItemFromParentDirectory(FileSystemItemSearchInDbDTO fileSystemItemToFind)
        {
            
            bool parentIsRootFolder = fileSystemItemToFind.Parent == null;
            if (parentIsRootFolder)
            {
                return await getFileItemFromRootDirectory(fileSystemItemToFind);
            }
            FileSystemItem? result = await _dbContext.FileSystemItems
                .Where(x => x.ParentId == fileSystemItemToFind.Parent.Id && x.Name == fileSystemItemToFind.Name && x.Type==fileSystemItemToFind.Type)
                .FirstOrDefaultAsync();
            return result;

        }

        public async Task MoveFileSystemItem(FileSystemItem fileToMove, FileSystemItem newParent)
        {
            fileToMove.Parent = newParent;
            await this._dbContext.SaveChangesAsync();

        }

        public async Task RemoveFileSystemItem(FileSystemItem fileToRemove)
        {
            this._dbContext.Remove(fileToRemove);
            await this._dbContext.SaveChangesAsync();
        }

        public async Task RenameFileSystemItem(FileSystemItem fileToEdit, String fileName)
        {
            fileToEdit.Name = fileName;
            await this._dbContext.SaveChangesAsync();
        }

        private async Task<List<FileSystemItem>> GetAllChildrensInFolderForRootParent(Guid ownerId)
        {
            return await _dbContext.FileSystemItems
                .Where(x => x.ParentId == null && x.OwnerId==ownerId)
                .ToListAsync();
        }



        private async Task<FileSystemItem?> getFileItemFromRootDirectory(FileSystemItemSearchInDbDTO fileItemDTO)
        {
            return await _dbContext.FileSystemItems
                .Where(x => x.ParentId == null && x.Name == fileItemDTO.Name && x.OwnerId==fileItemDTO.OwnerId && x.Type==fileItemDTO.Type)
                .FirstOrDefaultAsync();
        }
    }
}
