using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SimpleReport.Helpers;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;

namespace SimpleReport.Controllers.Api
{

    public class SubscriptionController : BaseApiController
    {
        private readonly IApiClient _apiClient;

        public SubscriptionController(IStorage reportStorage, ILogger logger, IApiClient apiClient) : base(reportStorage, logger)
        {
            _apiClient = apiClient;
        }

        [AcceptVerbs("GET")]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);

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
                _adminAccess.IsAllowedForMe(User);

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
        public async Task<IHttpActionResult> List(string filter = null, string reportId = null)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);

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
        public async Task<IHttpActionResult> Save(object subscription)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);

                var result = await _apiClient.Post("api/subscription/save", subscription);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Save", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Delete(Dictionary<string, object> obj)//this is really {id : int}, but webapi model binding is evil when it comes to simple values, hence we use this.
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);

                var result = await _apiClient.Post("api/subscription/delete", obj["Id"]);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Delete", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> Send(Dictionary<string, object> obj)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);

                var result = await _apiClient.Post("api/subscription/send", obj["Id"]);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in Send", ex);
                return InternalServerError();
            }
        }
    }
}