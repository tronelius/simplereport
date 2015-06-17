using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SimpleReport.Helpers;
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
                _logger.Error("Exception in All", ex);
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
                _logger.Error("Exception in Save", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Delete(Dictionary<string,object> obj)//this is really {id : int}, but webapi model binding is evil when it comes to simple values, hence we use this.
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);

                var result = await _apiClient.Post("api/schedule/delete", obj["Id"]);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Delete", ex);
                return InternalServerError();
            }
        }
    }
}