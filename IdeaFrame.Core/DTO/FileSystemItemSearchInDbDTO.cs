using IdeaFrame.Core.Domain.Entities;
using IdeaFrame.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class FileSystemItemSearchInDbDTO
    {
        public String Name;
        public FileSystemItem Parent;
        public FileItemType Type;
        public Guid OwnerId;

        public FileSystemItemSearchInDbDTO(string name, FileSystemItem parent, FileItemType type, Guid ownerId)
        {
            this.Name = name;
            this.Parent = parent;
            this.Type = type;
            this.OwnerId = ownerId;
        }
    }
}
