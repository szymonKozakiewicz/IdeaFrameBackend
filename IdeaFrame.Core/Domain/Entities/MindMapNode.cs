using IdeaFrame.Core.DTO;
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
        public string Color { get; set; } 
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public Guid FileId { get; set; }

        public MindMapNodeDTO ConvertToMindMapDTO()
        {
            return new MindMapNodeDTO
            {
                Id = Id.ToString(),
                Name = Name,
                Color = Color,
                Coordinates = new CoordinatesDTO(PositionX, PositionY)
            };

        }
    }
}
