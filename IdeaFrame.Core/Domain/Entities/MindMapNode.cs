using IdeaFrame.Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public Guid UiId { get; set; }

        public static MindMapNode BuildFromSaveDTO(MindMapNodeSaveDTO saveDTO,FileSystemItem file)
        {
           var newNode= new MindMapNode()
            {
                Name = saveDTO.Name,
                MindMapFile = file,
                PositionX = saveDTO.Coordinates.X,
                PositionY = saveDTO.Coordinates.Y,
                FileId = file.Id,
                Color = saveDTO.Color,
                UiId = Guid.Parse(saveDTO.UiId)
            };
            if(saveDTO.Id.Length > 0)
            {
                newNode.Id = Guid.Parse(saveDTO.Id);
            }
            return newNode;
        }
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
