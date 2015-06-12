using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using SimpleReport.Helpers;
using SimpleReport.Model;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;

namespace SimpleReport.Controllers.Api
{

    public class ScheduleController : BaseApiController
    {
        private readonly IApiClient _apiClient;

        public ScheduleController(IStorage reportStorage, ILogger logger, IApiClient apiClient) : base(reportStorage, logger)
        {
            _apiClient = apiClient;
        }

        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> All()
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                
                var result = await _apiClient.Get("api/schedule/all");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in GetviewModel", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Save(object schedule)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);

                var result = await _apiClient.Post("api/schedule/save", schedule);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in SaveReport", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Delete(dynamic obj)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);

                var result = await _apiClient.Post("api/schedule/delete", (object)obj.Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in DeleteReport", ex);
                return InternalServerError();
            }
        }
    }
}