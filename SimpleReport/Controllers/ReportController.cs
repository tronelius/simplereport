using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SimpleReport.Model;

namespace SimpleReport.Controllers
{
    public class ReportController : ApiController
    {
        ReportManager _reportManager;
        public ReportController(ReportManager reportManager)
        {
            _reportManager = reportManager;
        }

        public Report Get(Guid id)
        {
            return _reportManager.GetReport(id);
        } 
    }
}
