using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaFrame.Core.CustomValidationAttributes
{
    public class HasDigit : ValidationAttribute
    {
        public HasDigit():base("The field needs to have at least one digit")
        {
            
        }

        public override bool IsValid(object? value)
        {
            return base.IsValid(value);
        }
    }
}
