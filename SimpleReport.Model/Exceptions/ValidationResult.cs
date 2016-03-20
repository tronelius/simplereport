using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReport.Model.Exceptions
{
    public class ValidationResult
    {
        public ValidationResult(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
