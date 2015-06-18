using System;
using System.Collections.Generic;
using System.Linq;
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

            var errorResult = subscription.Validate();

            if (errorResult != null)
                return Json(new { Error = errorResult });

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

        [Route("list")]
        [HttpGet]
        public IHttpActionResult List(string filter)
        {
            try
            {
                _logger.Info("Getting all subscriptions for listing with filter:" + filter);
                var subs = _subscriptionRepository.List();

                if (filter == "failed")
                {
                    subs = subs.Where(x => x.Status == SubscriptionStatus.Failed).ToList();
                }

                var scheds = _scheduleRepository.List();
                var result = subs.Select(x => new {x.Id,  x.ReportId, Recipients = GetRecipients(x), x.Status, x.LastSent, Schedule = scheds.First(y => y.Id == x.ScheduleId).Name, x.ErrorMessage}).ToArray();

                return Json(result);
            }
            catch (Exception e)
            {
                _logger.Error("SubscriptionController.All", e);
                throw;
            }
        }

        private static string GetRecipients(Subscription x)
        {
            var recipients = new List<string>();

            if(!string.IsNullOrWhiteSpace(x.To))
                recipients.Add(x.To);

            if (!string.IsNullOrWhiteSpace(x.Cc))
                recipients.Add(x.Cc);

            if (!string.IsNullOrWhiteSpace(x.Bcc))
                recipients.Add(x.Bcc);

            return string.Join(";", recipients);
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
