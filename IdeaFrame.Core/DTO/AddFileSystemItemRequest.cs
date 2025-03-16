﻿using IdeaFrame.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class AddFileSystemItemRequest
    {
        [Required]
        public String Path { get; set; }
        [Required]
        [MaxLength(20)]
        public String Name { get; set; }
        [Required]
        public FileItemType Type { get; set; }

    }
}
