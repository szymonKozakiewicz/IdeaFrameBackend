using IdeaFrame.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class EditFileItemNameDTO : FileSystemItemDTO
    {
        [Required]
        public String NewName { get; set; }

    }
}
