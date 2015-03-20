using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleReport.Model;

namespace SimpleReport.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ReportManager _reportManager;
        public BaseController(ReportManager reportManager)
        {
            _reportManager = reportManager;
        }

        
    }
}