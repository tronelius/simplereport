using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Results;
using SimpleReport.Model;
using SimpleReport.Model.Storage;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers.Api
{
    //Todo dynamic acesscontrol needed.
    public class DesignerController : ApiController
    {
        private readonly IStorage _reportStorage;
        public DesignerController(IStorage reportstorage)
        {
            _reportStorage = reportstorage;
        }

        private void CheckForAdminAccess()
        {
            if (!User.IsInRole(_reportStorage.GetSettings().AdminAccess))
                throw new Exception("Action not allowed");
        }

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
                _reportStorage.SaveReport(reportToSave);
                return Ok();
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
                _reportStorage.SaveConnection(conn);
                return Ok();
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
                _reportStorage.SaveLookupReport(lrpt);
                return Ok();
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
                _reportStorage.SaveAccessList(acc);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }
    }
}