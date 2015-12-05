using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using SimpleReport.Helpers;
using SimpleReport.Model;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers.Api
{

    public class DesignerController : BaseApiController
    {
        private readonly IApiClient _apiClient;
        private readonly IApplicationSettings _applicationSettings;

        public DesignerController(IStorage reportStorage, ILogger logger, IApiClient apiClient, IApplicationSettings applicationSettings) : base(reportStorage, logger)
        {
            _apiClient = apiClient;
            _applicationSettings = applicationSettings;
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult GetViewModel()
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                DesignerViewModel vm = new DesignerViewModel(_reportStorage, User, _applicationSettings);
                return Ok(vm);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in GetviewModel", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> SaveReport([FromBody]Report reportToSave)
        {
            try {
                _adminAccess.IsAllowedForMe(User);
                Report currentReport = _reportStorage.GetReport(reportToSave.Id);
                
                if (_applicationSettings.SubscriptionEnabled && currentReport != null && currentReport.HasMailTemplateChanged(reportToSave)) {
                    var result = await _apiClient.Post("api/subscription/updatetemplate", new {reportguid = reportToSave.Id, subject = reportToSave.MailSubject, text = reportToSave.MailText});
                }

                _reportStorage.SaveReport(reportToSave);
                return Ok(reportToSave);
            } catch (Exception ex)
            {
                _logger.Error("Exception in SaveReport", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> DeleteReport([FromBody]Report rpt)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);

                if (_applicationSettings.SubscriptionEnabled)
                {
                    var result = await _apiClient.Get("api/subscription/hasSubscriptions?reportId=" + rpt.Id);

                    if ((bool) result)
                    {
                        return Ok(new DeleteInfo(false, "The report have subscriptions that must be removed first."));
                    }
                }

                var deleteinfo = _reportStorage.DeleteReport(rpt);
                return Ok(deleteinfo);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in DeleteReport", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult SaveConnection([FromBody]Connection conn)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                _reportStorage.SaveConnection(conn);
                return Ok(conn);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in SaveConnection", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult VerifyConnection([FromBody]Connection conn)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                var result = conn.VerifyConnectionstring();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in VerifyConnection", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult DeleteConnection([FromBody]Connection conn)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                var deleteinfo = _reportStorage.DeleteConnection(conn);
                return Ok(deleteinfo);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in DeleteConnection", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult SaveLookupReport([FromBody]LookupReport lrpt)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                _reportStorage.SaveLookupReport(lrpt);
                return Ok(lrpt);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in SaveLookupReport", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult DeleteLookupReport([FromBody]LookupReport lrpt)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                var deleteinfo = _reportStorage.DeleteLookupReport(lrpt);
                return Ok(deleteinfo);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in DeleteLookupReport", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult SaveAccessList([FromBody]Access acc)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                _reportStorage.SaveAccessList(acc);
                return Ok(acc);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in SaveAccessList", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult DeleteAccessList([FromBody]Access acc)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                var deleteinfo = _reportStorage.DeleteAccessList(acc);
                return Ok(deleteinfo);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in DeleteAccessList", ex);
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

    }
}