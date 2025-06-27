using IdeaFrame.Core.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.Domain.Entities
{
    public class MindMapBranch
    {
        [Key]
        public Guid Id { get; set; }
        public MindMapNode Target { get; set; }
        public MindMapNode Source { get; set; }

        public Guid TargetId { get; set; }
        public Guid SourceId { get; set; }


        public static MindMapBranch BuildFromSaveDTO(MindMapBranchSaveDTO saveDto)
        {
            var newBranch= new MindMapBranch()
            {
                TargetId = Guid.Parse(saveDto.Target.Id),
                SourceId = Guid.Parse(saveDto.Source.Id)
                
            };
            if(saveDto.Id.Length>0)
            {
                newBranch.Id = Guid.Parse(saveDto.Id);
            }
            return newBranch;
        }

        public BranchLoadDTO ConvertToBranchLoadDTO()
        {
            return new BranchLoadDTO()
            {
                Id = this.Id.ToString(),
                TargetId = this.TargetId.ToString(),
                SourceId = this.SourceId.ToString()
            };
            
     
        }

    }
}
