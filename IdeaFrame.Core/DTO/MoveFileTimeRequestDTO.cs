using IdeaFrame.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class MoveFileTimeRequestDTO: FileSystemItemDTO
    {

        [Required]
        public String NewPath { get; set; }

    }
}
