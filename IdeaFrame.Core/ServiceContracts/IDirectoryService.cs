using IdeaFrame.Core.Domain.Entities;
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
        public Task AddNewFolder(String newFolderName, String pathStr);


    }
}
