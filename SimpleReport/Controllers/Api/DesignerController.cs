using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Results;
using SimpleReport.Model;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers.Api
{

    public class DesignerController : BaseApiController
    {

        public DesignerController(IStorage reportStorage, ILogger logger) : base(reportStorage, logger){ }

        [AcceptVerbs("GET")]
        public IHttpActionResult GetViewModel()
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
                DesignerViewModel vm = new DesignerViewModel(_reportStorage, User);
                return Ok(vm);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in GetviewModel", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult SaveReport([FromBody]Report reportToSave)
        {
            try {
                _adminAccess.IsAllowedForMe(User);
                HandleNewEntity(reportToSave);
                _reportStorage.SaveReport(reportToSave);
                return Ok(reportToSave);
            } catch (Exception ex)
            {
                _logger.Error("Exception in SaveReport", ex);
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult DeleteReport([FromBody]Report rpt)
        {
            try
            {
                _adminAccess.IsAllowedForMe(User);
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
                HandleNewEntity(conn);
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
                HandleNewEntity(lrpt);
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