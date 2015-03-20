using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReport.Model
{
    public class ValidatableEntity
    {
        public bool IsValid()
        {
            return !this.Validate(new ValidationContext(this,null,null)).Any();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new ValidationResult[0];
        }
    }

    
}
