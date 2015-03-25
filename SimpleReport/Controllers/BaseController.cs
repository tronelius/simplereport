using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleReport.Model;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ReportManager _reportManager;
        public BaseController(ReportManager reportManager)
        {
            _reportManager = reportManager;
        }

        protected ReportViewModel GetReportViewModel()
        {
            ReportViewModel vm = new ReportViewModel();
            vm.Reports = _reportManager.GetReports();
            return vm;
        }
        
    }
}