using System;
using Quartz;
using Worker.Common.Common;

namespace WorkerHost.Jobs
{
    [DisallowConcurrentExecution]
    public class SubscriptionJob : IJob
    {
        private readonly ILogger _logger;

        public SubscriptionJob(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("Task running now!!");
            Console.WriteLine("Hello, im running once a minute:"  + DateTime.Now.ToShortTimeString());
        }
    }
}