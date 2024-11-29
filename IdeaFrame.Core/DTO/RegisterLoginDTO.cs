using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.DTO
{
    public class RegisterLoginDTO
    {
        [Required]
        [StringLength(100,MinimumLength =6)]
        public string? Login { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string? Password { get; set; }

    }
}
