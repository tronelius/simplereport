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
                CheckForAdminAccess();
                DesignerViewModel vm = new DesignerViewModel(_reportStorage, User);
                return Ok(vm);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult SaveReport([FromBody]Report reportToSave)
        {
            try {
                CheckForAdminAccess();
                HandleNewEntity(reportToSave);
                _reportStorage.SaveReport(reportToSave);
                return Ok(reportToSave);
            } catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult SaveConnection([FromBody]Connection conn)
        {
            try
            {
                CheckForAdminAccess();
                HandleNewEntity(conn);
                _reportStorage.SaveConnection(conn);
                return Ok(conn);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult SaveLookupReport([FromBody]LookupReport lrpt)
        {
            try
            {
                CheckForAdminAccess();
                HandleNewEntity(lrpt);
                _reportStorage.SaveLookupReport(lrpt);
                return Ok(lrpt);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [AcceptVerbs("POST")]
        public IHttpActionResult SaveAccessList([FromBody]Access acc)
        {
            try
            {
                CheckForAdminAccess();
                HandleNewEntity(acc);
                _reportStorage.SaveAccessList(acc);
                return Ok(acc);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }
    }
}