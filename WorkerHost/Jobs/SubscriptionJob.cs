using System;
using System.Configuration;
using System.Linq;
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
        private readonly IApplicationSettings _applicationSettings;
        private const int MaxFailedAttempts = 5;

        public SubscriptionJob(ILogger logger, ISubscriptionRepository subscriptionRepository, IScheduleRepository scheduleRepository, IWorkerApiClient workerApiClient, IMailSender mailSender, IApplicationSettings applicationSettings)
        {
            _logger = logger;
            _subscriptionRepository = subscriptionRepository;
            _scheduleRepository = scheduleRepository;
            _workerApiClient = workerApiClient;
            _mailSender = mailSender;
            _applicationSettings = applicationSettings;
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.Trace("SubscriptionJob running now!!");

                var subs = _subscriptionRepository.GetSubscriptionsWithSendDateBefore(DateTime.Now, MaxFailedAttempts);

                var alreadyFailed = subs.Where(IsFailed).ToArray();
                Parallel.ForEach(subs, SendSubscription);
                var justFailed = subs.Where(IsFailed).Except(alreadyFailed).ToArray();

                if (justFailed.Any())
                {
                    _logger.Warn("The following new subscriptions failed:"+ string.Join(",", justFailed.Select(x => x.Id)));
                    SendFailedSubscriptionEmail(justFailed);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Could not execute subscription job", ex);
            }
        }

        private void SendFailedSubscriptionEmail(Subscription[] justFailed)
        {
            var body = "The following subscriptions just failed:" + Environment.NewLine;
            body += string.Join(Environment.NewLine, justFailed.Select(x => "Subscription " +   x.Id + " with subject " + x.MailSubject + " failed due to: " + x.ErrorMessage));

            _mailSender.Send("Failed subscriptions", body, _applicationSettings.ErrorMailRecipient);
        }

        private static bool IsFailed(Subscription sub)
        {
            return sub.Status == SubscriptionStatus.Failed || sub.Status == SubscriptionStatus.Suspended;
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
                        _mailSender.Send(subscription.MailSubject, subscription.MailText, subscription.To, subscription.Cc, subscription.Bcc, reportData);
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
                    if (subscription.FailedAttempts == MaxFailedAttempts)
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