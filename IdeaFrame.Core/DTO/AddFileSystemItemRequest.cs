using IdeaFrame.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class AddFileSystemItemRequest
    {
        public String Path { get; set; }
        public String Name { get; set; }
        public FileItemType Type { get; set; }

    }
}
