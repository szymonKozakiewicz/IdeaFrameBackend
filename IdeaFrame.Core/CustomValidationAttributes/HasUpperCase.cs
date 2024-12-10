using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.CustomValidationAttributes
{
    public class HasUpperCase:ValidationAttribute
    {
        public HasUpperCase():base("The field need to have one uppercase latter")
        {
            
        }

        public override bool IsValid(object? value)
        {
            return base.IsValid(value);
        }
    }
}
