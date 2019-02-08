using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using SimpleReport.Helpers;
using SimpleReport.Model;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;
using SimpleReport.Model.Subscriptions;
using SimpleReport.ViewModel;
using IApplicationSettings = SimpleReport.Model.IApplicationSettings;

namespace SimpleReport.Controllers.Api
{

    public class DesignerController : BaseApiController
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly IStorageHelper _storageHelper;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public DesignerController(IStorage reportStorage, ILogger logger,  IApplicationSettings applicationSettings, IStorageHelper storageHelper, ISubscriptionRepository subscriptionRepository) : base(reportStorage, logger)
        {
            _applicationSettings = applicationSettings;
            _storageHelper = storageHelper;
            _subscriptionRepository = subscriptionRepository;
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
                    var subs =_subscriptionRepository.GetSubscriptionsByReportId(rpt.Id);
                    if (subs.Count > 0)
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
        public IHttpActionResult SaveTypeAheadReport([FromBody]TypeAheadReport rpt)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                _reportStorage.SaveTypeAheadReport(rpt);
                return Ok(rpt);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in SaveTypeAheadReport", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult DeleteTypeAheadReport([FromBody]TypeAheadReport rpt)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                var deleteinfo = _reportStorage.DeleteTypeAheadReport(rpt.Id);
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

        [AcceptVerbs("GET")]
        public HttpResponseMessage ExportModel()
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                var model = _reportStorage.LoadModel();
                HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
                MemoryStream ms = new MemoryStream();
                _storageHelper.WriteModelToStream(model, ms);
                result.Content = new ByteArrayContent(ms.ToArray());
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = "datamodel.json"
                };
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in ExportModel", ex);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [AcceptVerbs("POST")]
        public async Task<IHttpActionResult> ImportModel()
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                if (provider.Contents.Count>=1)
                {
                    var file = provider.Contents[0];
                    var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                    var buffer = await file.ReadAsStreamAsync();
                    var importModel = _storageHelper.ReadModelFromStream(buffer);
                    importModel.Settings.AddCurrentUserToAdminAccess(User);
                    var errors = importModel.RemoveIllegalItemsInModel();
                    _reportStorage.SaveModel(importModel);
                    if  (errors.Count > 0)
                        return Ok(new {message=errors});
                    return Ok();
                }
                return InternalServerError();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in ImportModel", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult ClearModel()
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                _reportStorage.ClearModel();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in ClearModel", ex);
                return InternalServerError();
            }
        }

    }
}