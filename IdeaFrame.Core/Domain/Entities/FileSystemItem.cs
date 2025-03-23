using IdeaFrame.Core.Domain.Entities.IdentitiesEntities;
using IdeaFrame.Core.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Domain.Entities
{
    public class FileSystemItem
    {
        public String Name { get; set; }
  
        public Guid Id;
        public FileItemType Type { get; set; }
        public Guid? ParentId { get; set; }
        public FileSystemItem? Parent { get; set; }
        public ApplicationUser Owner { get; set; }
        public Guid OwnerId { get; set; }
        public FileSystemItem()
        {
            
        }
        public FileSystemItem(FileSystemItem? parent, FileItemType type, String name,Guid ownerId)
        {
            Id= Guid.NewGuid();
            Parent = parent;
            if(parent != null)
            {
                ParentId = parent.Id;
            }
            Type = type;
            Name = name;
            OwnerId = ownerId;
        }


    }
}
