using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleReport.Model.Logging
{
    public interface ILogger
    {
        void Trace(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception ex = null);
    }
}
