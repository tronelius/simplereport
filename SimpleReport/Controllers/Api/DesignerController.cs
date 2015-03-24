using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using SimpleReport.Model;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers.Api
{
    public class DesignerController : ApiController
    {
        private readonly ReportManager _reportManager;
        public DesignerController(ReportManager reportManager)
        {
            _reportManager = reportManager;
        }

        [AcceptVerbs("GET")]
        public IHttpActionResult Reports()
        {
            try
            {
                DesignerViewModel vm = new DesignerViewModel();
                vm.Reports = _reportManager.GetReports().ToList();
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
                _reportManager.SaveReport(reportToSave);
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
                _reportManager.SaveConnection(conn);
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
                _reportManager.SaveLookupReport(lrpt);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }
    }
}