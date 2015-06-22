using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Worker.Common.Api;
using Worker.Common.Common;
using Worker.Common.Model;
using Worker.Common.Repository;
using Worker.Common.Service;

namespace WorkerHost.Jobs
{
    [DisallowConcurrentExecution]
    public class SubscriptionJob : IJob
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IWorkerApiClient _workerApiClient;
        private readonly IMailSender _mailSender;

        public SubscriptionJob(ILogger logger, ISubscriptionRepository subscriptionRepository, IScheduleRepository scheduleRepository, IWorkerApiClient workerApiClient, IMailSender mailSender)
        {
            _logger = logger;
            _subscriptionRepository = subscriptionRepository;
            _scheduleRepository = scheduleRepository;
            _workerApiClient = workerApiClient;
            _mailSender = mailSender;
        }

        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("Task running now!!");
            Console.WriteLine("Hello, im running once a minute:"  + DateTime.Now.ToShortTimeString());
            
            var subs = _subscriptionRepository.GetSubscriptionsWithSendDateBefore(DateTime.Now).Take(1); //TODO: remove take. used for debugging

            Parallel.ForEach(subs, SendSubscription);
        }

        private void SendSubscription(Subscription subscription)
        {
            _logger.Info("Sending sub " + subscription.Id +" on thread " + Thread.CurrentThread.ManagedThreadId);
            try
            {
                var schedule = _scheduleRepository.Get(subscription.ScheduleId);

                subscription.Status = SubscriptionStatus.Ongoing;
                _subscriptionRepository.Update(subscription);

                var reportData = _workerApiClient.GetExcelReport(subscription.ReportParams);
                _mailSender.Send(subscription.To, subscription.Cc, subscription.Bcc, subscription.MailSubject, subscription.MailText, reportData);

                subscription.Status = SubscriptionStatus.Success;
                subscription.SetNextSendDate(schedule.Cron);
                subscription.LastSent = DateTime.Now;
                subscription.FailedAttempts = null;
            }
            catch (Exception e)
            {
                _logger.Error("Failed to send subscription with id " + subscription.Id, e);
                subscription.Status = SubscriptionStatus.Failed;
                subscription.ErrorMessage = e.Message;
                subscription.LastErrorDate = DateTime.Now;
                subscription.FailedAttempts = subscription.FailedAttempts.HasValue ? subscription.FailedAttempts++ : 1;
            }
            finally
            {
                _subscriptionRepository.Update(subscription);
            }

            _logger.Info("Finished with sub " + subscription.Id + " on thread " + Thread.CurrentThread.ManagedThreadId);
        }
    }
}