using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleReport.Model.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}
