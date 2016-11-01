using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleReport.Model.Logging
{
    public class Nlogger : ILogger
    {

        public void Trace(string message)
        {
            NLog.LogManager.GetCurrentClassLogger().Trace(message);
        }
        public void Info(string message)
        {
            NLog.LogManager.GetCurrentClassLogger().Info(message);
        }

        public void Warn(string message)
        {
            NLog.LogManager.GetCurrentClassLogger().Warn(message);
        }

        public void Error(string message, Exception ex = null)
        {
            if (ex != null)
                NLog.LogManager.GetCurrentClassLogger().Error(ex,message);
            else
                NLog.LogManager.GetCurrentClassLogger().Error(message);
        }
    }
}
