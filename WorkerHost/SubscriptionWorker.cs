using System;
using Microsoft.Owin.Hosting;
using Ninject;
using Quartz;
using Quartz.Impl;
using SimpleReport.Model.Logging;
using WorkerHost.Config;
using WorkerHost.Jobs;
using IoCConfig = WorkerHost.Config.IoCConfig;

namespace WorkerHost
{
    public class SubscriptionWorker
    {
        protected IScheduler _scheduler;

        public void Start()
        {
            
            StartJobs();
        }

        private void StartJobs()
        {
            var kernel = new StandardKernel();
            IoCConfig.Register(kernel);

            try
            {
                // Create a scheduler and give it the Ninject job factory created earlier
                _scheduler = new StdSchedulerFactory().GetScheduler();
                _scheduler.JobFactory = new NinjectJobFactory(kernel);
                _scheduler.Start();
                IJobDetail job = JobBuilder.Create<SubscriptionJob>().Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .WithDailyTimeIntervalSchedule
                    (s =>
                        s.WithIntervalInMinutes(1)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                    ).WithIdentity("OnceAMinuteNoConcurrency")
                    .Build();

                _scheduler.ScheduleJob(job, trigger);
                _scheduler.TriggerJob(job.Key);//run immediately
            } catch (Exception ex)
            {
                var logger = kernel.Get<ILogger>();
                logger.Error("Could not start subscription workers",ex);
            }
        }
       

        public void Stop()
        {
            _scheduler.Shutdown();
        }
    }
}
