using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.RepositoryContracts;
using IdeaFrame.Infrastructure.DbContextCustom;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Infrastructure.Repositories
{
    public class DirectoryRepository : IDirectoryRepository
    {

        private MyDbContexSqlServer _context;
        public DirectoryRepository(MyDbContexSqlServer _context)
        {
            this._context = _context;
        }

        public async Task AddNewFileSystemItem(FileSystemItem newFolder)
        {
           await this._context.AddAsync(newFolder);
           await this._context.SaveChangesAsync();
        }

        public async Task<List<FileSystemItem>> GetAllChildrensInFolder(FileSystemItem parent)
        {
            bool parentIsRootFolder = parent == null;
            if (parentIsRootFolder)
            {
                return await GetAllChildrensInFolderForRootParent();
            }
            List<FileSystemItem>result=await _context.FileSystemItems
                .Where(x=>x.ParentId==parent.Id)
                .ToListAsync();
            return result;
        }

        public async Task<FileSystemItem?> GetFileItemFromParentDirectory(FileSystemItem? parent, string fileItemName)
        {
            bool parentIsRootFolder = parent == null;
            if (parentIsRootFolder)
            {
                return await getFileItemFromRootDirectory(fileItemName);
            }
            FileSystemItem? result = await _context.FileSystemItems
                .Where(x => x.ParentId == parent.Id && x.Name == fileItemName)
                .FirstOrDefaultAsync();
            return result;

        }

        private async Task<List<FileSystemItem>> GetAllChildrensInFolderForRootParent()
        {
            return await _context.FileSystemItems
                .Where(x => x.ParentId == null)
                .ToListAsync();
        }



        private async Task<FileSystemItem?> getFileItemFromRootDirectory(string fileItemName)
        {
            return await _context.FileSystemItems
                .Where(x => x.ParentId == null && x.Name == fileItemName)
                .FirstOrDefaultAsync();
        }
    }
}
