using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Owin;
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

                    var newSyncedDate = DateTime.Now;
                    var oldSyncedDate = subscription.SyncedDate ?? DateTime.Now;
                    var reportParams = subscription.ReportParams;

                    //there are basically here so that we can run the report with known dates. like, we want everything that happened since last sync to now, where now is a known date so that if we run the query again an hour later, we known exactly where we left off
                    reportParams = reportParams.Replace("=SyncedDate", "=" + oldSyncedDate.ToString("s"));
                    reportParams = reportParams.Replace("=SyncedRunningDate", "=" + newSyncedDate.ToString("s"));
                    _logger.Info("Executing report with parameters= "+reportParams);

                    var reportResult = _workerApiClient.GetReport(reportParams);
                    if (reportResult == null)
                    {
                        _logger.Info("Empty report for subscription with id " + subscription.Id);
                        if (subscription.SendEmptyEmails)
                        {
                            _mailSender.Send(subscription.MailSubject, subscription.MailText, subscription.To, subscription.Cc, subscription.Bcc, null, null);
                            _logger.Info("Sent empty email for subscription with id " + subscription.Id);
                            subscription.LastSent = DateTime.Now;
                        }
                    }
                    else if (reportResult.Data.Length > 0)
                    {
                        _mailSender.Send(subscription.MailSubject, subscription.MailText, subscription.To, subscription.Cc, subscription.Bcc, reportResult.Data, reportResult.FileName);
                        subscription.LastSent = DateTime.Now;
                    }
                    
                    subscription.Status = SubscriptionStatus.Success;
                    subscription.SyncedDate = newSyncedDate;
                    subscription.SetNextSendDate(schedule.Cron);
                    subscription.FailedAttempts = 0;
                    subscription.ErrorMessage = "";
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
                    if (subscription.SubscriptionType == SubscriptionTypeEnum.OneTime && subscription.Status == SubscriptionStatus.Success)
                    {
                        _subscriptionRepository.Delete(subscription.Id);
                    }
                    else
                    {
                        subscription.LastRun = DateTime.Now;
                        _subscriptionRepository.Update(subscription);
                    }
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