using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
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
            _logger.Trace("Creating schedule: " + JsonConvert.SerializeObject(subscription));

            var errorResult = subscription.Validate();

            if (errorResult != null)
                return Json(new { Error = errorResult });

            var schedule = _scheduleRepository.Get(subscription.ScheduleId);
            subscription.SetNextSendDate(schedule.Cron);

            if (subscription.Id == 0)
            {
                var id = _subscriptionRepository.Insert(subscription);
                return Json(new { Id = id });
            }
            
            if (subscription.Status == SubscriptionStatus.Suspended)
            {
                subscription.FailedAttempts = 0;
                subscription.Status = SubscriptionStatus.NotSet;
            }

            _subscriptionRepository.Update(subscription);
            return Json(new { Id = subscription.Id });
        }

        [Route("delete")]
        [HttpPost]
        public IHttpActionResult Delete([FromBody]int id)
        {
            _logger.Trace("Deleting subscription: " + id);

            _subscriptionRepository.Delete(id);
            var result = _subscriptionRepository.List();
            return Json(result);
        }

        [Route("send")]
        [HttpPost]
        public IHttpActionResult Send([FromBody]int id)
        {
            _logger.Trace("Set send on subscription: " + id);

            _subscriptionRepository.SendNow(id);
            var result = _subscriptionRepository.List();
            return Json(result);
        }

        [Route("all")]
        [HttpGet]
        public IHttpActionResult All()
        {
            try
            {
                _logger.Trace("Getting all subscription");
                var result = _subscriptionRepository.List();
                return Json(result);
            }
            catch (Exception e)
            {
                _logger.Error("SubscriptionController.All", e);
                throw;
            }
        }

        [Route("get")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                _logger.Trace("Getting subscription:" + id);
                var result = _subscriptionRepository.Get(id);
                return Json(result);
            }
            catch (Exception e)
            {
                _logger.Error("SubscriptionController.Get", e);
                throw;
            }
        }

        [Route("updatetemplate")]
        [HttpPost]
        public IHttpActionResult UpdateTemplate([FromBody]UpdateTemplateText updateTemplateText)
        {
            try
            {
                _logger.Trace("Update template text for all subscription to report");
                 _subscriptionRepository.UpdateTemplateText(updateTemplateText);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.Error("SubscriptionController.UpdateTemplate", e);
                throw;
            }

        }

        [Route("list")]
        [HttpGet]
        public IHttpActionResult List(string filter, string reportId)
        {
            try
            {
                _logger.Trace("Getting all subscriptions for listing with filter:" + filter);
                var subs = _subscriptionRepository.List();

                //TODO: do we want a more complex filtering? would be nice to merge filter and reportid and have some kind of dsl for it?
                if (filter == "failed")
                {
                    subs = subs.Where(x => x.Status == SubscriptionStatus.Failed || x.Status == SubscriptionStatus.Suspended).ToList();
                }

                if (!string.IsNullOrEmpty(reportId))
                {
                    var id = Guid.Parse(reportId);
                    subs = subs.Where(x => x.ReportId == id).ToList();
                }

                var scheds = _scheduleRepository.List();
                var result = subs.Select(x => new {x.Id,  x.ReportId, x.To, x.Cc,x.Bcc, Status = x.Status.ToString(), x.LastSent, Schedule = scheds.First(y => y.Id == x.ScheduleId).Name, x.ErrorMessage, x.ReportParams}).ToArray();

                return Json(result);
            }
            catch (Exception e)
            {
                _logger.Error("SubscriptionController.All", e);
                throw;
            }
        }

        [Route("hasSubscriptions")]
        [HttpGet]
        public IHttpActionResult List(string reportId)
        {
            try
            {
                _logger.Trace("Checking if report has subscriptions:" + reportId);
                var subs = _subscriptionRepository.List();
                var id = Guid.Parse(reportId);
                var hasSubs = subs.Any(x => x.ReportId == id);
                
                return Json(hasSubs);
            }
            catch (Exception e)
            {
                _logger.Error("SubscriptionController.All", e);
                throw;
            }
        }
    }
}
