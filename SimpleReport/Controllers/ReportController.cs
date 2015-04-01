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
        ReportResolver ReportResolver;
        public ReportController(ReportResolver reportResolver)
        {
            ReportResolver = reportResolver;
        }

        public Report Get(Guid id)
        {
            return ReportResolver.GetReport(id);
        } 
    }
}
