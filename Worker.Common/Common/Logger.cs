using System;
using NLog;

namespace Worker.Common.Common
{
    public class Logger : ILogger
    {
        public void Trace(string message)
        {
            LogManager.GetCurrentClassLogger().Trace(message);
        }

        public void Info(string message)
        {
            LogManager.GetCurrentClassLogger().Info(message);
        }

        public void Warn(string message)
        {
            LogManager.GetCurrentClassLogger().Warn(message);
        }

        public void Error(string message, Exception ex = null)
        {
            if (ex != null)
                LogManager.GetCurrentClassLogger().ErrorException(message, ex);
            else
                LogManager.GetCurrentClassLogger().Error(message);
        }
    }

    public interface ILogger
    {
        void Trace(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception ex = null);
    }
}
