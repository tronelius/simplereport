using System;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web.Http;
using NCrontab;
using Newtonsoft.Json;
using Worker.Common.Common;
using Worker.Common.Model;
using Worker.Common.Repository;

namespace WorkerWebApi.Controllers
{
    [RoutePrefix("api/subscription")]
    public class SubscriptionController : ApiController
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public SubscriptionController(ILogger logger, ISubscriptionRepository subscriptionRepository, IScheduleRepository scheduleRepository)
        {
            _logger = logger;
            _subscriptionRepository = subscriptionRepository;
            _scheduleRepository = scheduleRepository;
        }

        [Route("save")]
        [HttpPost]
        public IHttpActionResult Save(Subscription subscription)
        {
            _logger.Info("Creating schedule: " + JsonConvert.SerializeObject(subscription));

            var errorResult = ValidateSubscription(subscription);

            if (errorResult != null)
                return errorResult;

            SetNextSendDate(subscription);

            if (subscription.Id == 0)
            {
                var id = _subscriptionRepository.Insert(subscription);
                return Json(new { Id = id });
            }
            else
            {
                _subscriptionRepository.Update(subscription);
                return Json(new { Id = subscription.Id });
            }
        }

        private IHttpActionResult ValidateSubscription(Subscription subscription)
        {
            if (string.IsNullOrWhiteSpace(subscription.To + subscription.Cc + subscription.Bcc))
                return Json(new { Error = "At least one recipient must be defined" });

            var emails = subscription.To + ";" + subscription.Cc + ";" + subscription.Bcc;
            string[] allToAddresses = emails.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string toAddress in allToAddresses)
            {
                try
                {
                    new MailAddress(toAddress);
                }
                catch (FormatException)
                {
                    return Json(new { Error = "Malformed email-recipients" });
                }
            }

            return null;
        }

        [Route("delete")]
        [HttpPost]
        public IHttpActionResult Delete([FromBody]int id)
        {
            _logger.Info("Deleting subscription: " + id);

            _subscriptionRepository.Delete(id);
            var result = _subscriptionRepository.List();
            return Json(result);
        }

        [Route("all")]
        [HttpGet]
        public IHttpActionResult All()
        {
            try
            {
                _logger.Info("Getting all subscription");
                var result = _subscriptionRepository.List();
                return Json(result);
            }
            catch (Exception e)
            {
                _logger.Error("SubscriptionController.All", e);
                throw;
            }
        }

        private void SetNextSendDate(Subscription subscription)
        {
            var schedule = _scheduleRepository.Get(subscription.ScheduleId);
            var crons = schedule.Cron.Split(';'); //we can have composite crons, separated by ;
            var date = crons.Select(CrontabSchedule.Parse).Select(x => x.GetNextOccurrence(DateTime.Now)).Min();

            subscription.NextSend = date;
        }
    }
}
