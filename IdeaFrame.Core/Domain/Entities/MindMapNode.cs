using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Domain.Entities
{
    public class MindMapNode
    {

        public Guid Id { get; set; }
        public string Name { get; set; }
        public FileSystemItem MindMapFile { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
        public Guid FileId { get; set; }
    }
}
