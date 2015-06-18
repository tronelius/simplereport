using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SimpleReport.Helpers;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;

namespace SimpleReport.Controllers.Api
{

    public class ReportIdWrapper
    {
        public string ReportId { get; set; }
        public object Data { get; set; }
    }

    public class SubscriptionController : BaseApiController
    {
        private readonly IApiClient _apiClient;

        public SubscriptionController(IStorage reportStorage, ILogger logger, IApiClient apiClient) : base(reportStorage, logger)
        {
            _apiClient = apiClient;
        }

        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> Get(string reportId, int id)
        {
            try
            {
                CheckAccess(reportId);

                var result = await _apiClient.Get("api/subscription/get?id=" + id);
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

                var result = await _apiClient.Get("api/subscription/all");
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

                var result = await _apiClient.Get("api/subscription/list?filter=" + filter + "&reportId=" + reportId);
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
                report.IsAllowedToEditSubscriptions(User, _adminAccess);
            }
            else
            {
                _adminAccess.IsAllowedForMe(User);
            }
        }
    }
}