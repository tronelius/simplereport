using System;
using System.Web.Http;
using SimpleReport.Model;

namespace SimpleReport.Controllers.Api
{
    public class DesignerController : ApiController
    {
        private readonly ReportManager _reportManager;
        public DesignerController(ReportManager reportManager)
        {
            _reportManager = reportManager;
        }

        public DesignerController()
        {
            
        }


        [AcceptVerbs("POST")]
        public IHttpActionResult Save([FromBody]Report reportToSave)
        {
            try { 
                _reportManager.SaveReport(reportToSave);
                return Ok();
            } catch (Exception ex)
            {
                return InternalServerError();
            }
        }
    }
}