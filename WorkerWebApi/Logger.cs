using System;
using NLog;

namespace WorkerWebApi
{
    public class Logger : ILogger
    {
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
        void Info(string message);
        void Warn(string message);
        void Error(string message, Exception ex = null);
    }
}
