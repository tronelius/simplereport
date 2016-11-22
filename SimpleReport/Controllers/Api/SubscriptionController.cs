using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SimpleReport.Helpers;
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
        private readonly IApiClient _apiClient;

        public SubscriptionController(IStorage reportStorage, ILogger logger, ISubscriptionRepository subscriptionRepository, IScheduleRepository scheduleRepository, IApiClient apiClient) : base(reportStorage, logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _scheduleRepository = scheduleRepository;
            _apiClient = apiClient;
        }

        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> Get(string reportId, int id)
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

                var scheds = _scheduleRepository.List();
                var result = subs.Select(x => new { x.Id, x.ReportId, x.To, x.Cc, x.Bcc, Status = x.Status.ToString(), x.LastSent, Schedule = scheds.First(y => y.Id == x.ScheduleId).Name, x.ErrorMessage, x.ReportParams }).ToArray();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in List", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Save(ReportIdWrapper reportIdWrapper)
        {
            try
            {
                CheckAccess(reportIdWrapper.ReportId);

                var result = await _apiClient.Post("api/subscription/save", reportIdWrapper.Data);
                return Ok(result);
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

                var result = await _apiClient.Post("api/subscription/delete", reportIdWrapper.Data);
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

                var result = await _apiClient.Post("api/subscription/send", reportIdWrapper.Data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Send", ex);
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
                _adminAccess.IsAllowedForMe(User);
            }
        }
    }
}