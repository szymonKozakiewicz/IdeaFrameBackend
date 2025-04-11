using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.ServiceContracts
{
    public interface IDirectoryService
    {
        public Task AddNewFileItem(FileSystemItemDTO fileSystemRequest);
        public Task<bool> IsNameAvailable(FileSystemItemDTO fileSystemRequest);

        public Task<List<FileSystemItem>> GetAllChildrensInPath(String path);

        public Task RemoveFileItem(FileSystemItemDTO fileToRemove);

        public Task MoveFileItem(MoveFileTimeRequestDTO fileToMove);

        public Task EditFileItemName(EditFileItemNameDTO editFileItem);

    }
}
