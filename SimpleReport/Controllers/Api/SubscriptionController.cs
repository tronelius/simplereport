using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SimpleReport.Helpers;
using SimpleReport.Model;
using SimpleReport.Model.Exceptions;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;
using SimpleReport.Model.Subscriptions;

namespace SimpleReport.Controllers.Api
{
    public class SubscriptionController : BaseApiController
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public SubscriptionController(IStorage reportStorage, ILogger logger, ISubscriptionRepository subscriptionRepository, IScheduleRepository scheduleRepository) : base(reportStorage, logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _scheduleRepository = scheduleRepository;
        }

        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> Get(string reportId, Guid id)
        {
            try
            {
                CheckAccess(reportId);
                var result = _subscriptionRepository.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Get", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> All()
        {
            try
            {
                CheckAccess(null);
                var result = _subscriptionRepository.List();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in All", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> List(string reportId = null, string filter = null)
        {
            try
            {
                CheckAccess(reportId);
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

                var scheds = _scheduleRepository.ListAll();
                var result = subs.Select(x => new { x.Id, x.ReportId, x.To, x.Cc, x.Bcc, Status = x.Status.ToString(), x.LastSent, Schedule = scheds.FirstOrDefault(y => y.Id == x.ScheduleId)?.Name, x.ErrorMessage, x.ReportParams, x.MailSubject }).ToArray();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in List", ex);
                return InternalServerError();
            }
        }
        
        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> GetSettings()
        {
            try
            {
                CheckAccess(null);
                var result = _reportStorage.GetSettings();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in All", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Save(ReportIdWrapper reportIdWrapper)
        {
            try
            {
                CheckAccess(reportIdWrapper.ReportId);
                Subscription subscription = JsonConvert.DeserializeObject<Subscription>(reportIdWrapper.Data.ToString());
                _logger.Trace("Creating subscription: " + JsonConvert.SerializeObject(reportIdWrapper.Data));

                var errorResult = subscription.Validate();
                if (errorResult != null)
                    return Json(new { Error = errorResult });

                Schedule schedule = null;
                if (subscription.SubscriptionType == SubscriptionTypeEnum.OneTime)
                {
                    schedule = _scheduleRepository.GetOneTimeSchedule();
                    subscription.ScheduleId = schedule.Id;
                }
                else
                {
                    schedule = _scheduleRepository.Get(subscription.ScheduleId);
                }
                subscription.SetNextSendDate(schedule.Cron);

                if (subscription.Id == Guid.Empty)
                {
                    subscription.Id = new Guid();
                    var id = _subscriptionRepository.Insert(subscription);
                    return Json(new { Id = id });
                }

                if (subscription.Status == SubscriptionStatus.Suspended)
                {
                    subscription.FailedAttempts = 0;
                    subscription.Status = SubscriptionStatus.NotSet;
                }
                _subscriptionRepository.Update(subscription);
                return Ok(new { Id = subscription.Id });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Save", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Delete(ReportIdWrapper reportIdWrapper)
        {
            try
            {
                CheckAccess(reportIdWrapper.ReportId);
                _logger.Trace("Deleting subscription: " + reportIdWrapper.Data);
                Guid id = new Guid(reportIdWrapper.Data.ToString());
                _subscriptionRepository.Delete(id);
                var result = _subscriptionRepository.List();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Delete", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Send(ReportIdWrapper reportIdWrapper)
        {
            try
            {
                CheckAccess(reportIdWrapper.ReportId);
                _logger.Trace("Set send on subscription: " + reportIdWrapper.Data);
                Guid id = new Guid(reportIdWrapper.Data.ToString());
                _subscriptionRepository.SendNow(id);

                var result = _subscriptionRepository.List();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Send", ex);
                return InternalServerError();
            }
        }
        
        [AcceptVerbs("POST")]
        public IHttpActionResult SaveSettings([FromBody]Settings settings)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                _reportStorage.SaveSettings(settings);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in SaveSettings", ex);
                return InternalServerError();
            }
        }

        private void CheckAccess(string reportId)
        {
            if (!string.IsNullOrEmpty(reportId))
            {
                var report = _reportStorage.GetReport(Guid.Parse(reportId));
                if (report == null)
                    throw new EntityNotFoundException("Report not found");

                report.IsAllowedToEditSubscriptions(User, _adminAccess);
            }
            else
            {
                if (!_subscriptionAccess.IsAllowedToSeeSubscriptions(User))
                {
                    _adminAccess.IsAllowedForMe(User);
                }
            }
        }
    }
}