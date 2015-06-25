using System;
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
            _logger.Trace("SubscriptionJob running now!!");

            var subs = _subscriptionRepository.GetSubscriptionsWithSendDateBefore(DateTime.Now);

            Parallel.ForEach(subs, SendSubscription);
        }

        private void SendSubscription(Subscription subscription)
        {
            _logger.Trace("Sending sub " + subscription.Id);
            try
            {
                try
                {
                    var schedule = _scheduleRepository.Get(subscription.ScheduleId);

                    subscription.Status = SubscriptionStatus.Ongoing;
                    _subscriptionRepository.Update(subscription);

                    var reportData = _workerApiClient.GetExcelReport(subscription.ReportParams);
                    if (reportData != null && reportData.Length > 0 || subscription.SendEmptyEmails)
                    {
                        _mailSender.Send(subscription.To, subscription.Cc, subscription.Bcc, subscription.MailSubject, subscription.MailText, reportData);
                        subscription.LastSent = DateTime.Now;
                    }
                    
                    subscription.Status = SubscriptionStatus.Success;
                    subscription.SetNextSendDate(schedule.Cron);
                    subscription.FailedAttempts = 0;
                }
                catch (Exception e)
                {
                    _logger.Error("Failed to send subscription with id " + subscription.Id, e);
                    subscription.Status = SubscriptionStatus.Failed;
                    subscription.ErrorMessage = e.Message;
                    subscription.LastErrorDate = DateTime.Now;
                    subscription.FailedAttempts++;
                    if(subscription.FailedAttempts == 5)
                        subscription.Status = SubscriptionStatus.Suspended;
                }
                finally
                {
                    subscription.LastRun = DateTime.Now;
                    _subscriptionRepository.Update(subscription);
                }
            }
            catch (Exception e)
            {
                _logger.Error("SendSubscription", e);
            }

            _logger.Trace("Finished with sub " + subscription.Id);
        }
    }
}