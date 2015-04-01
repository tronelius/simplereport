using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleReport.Model;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ReportResolver _reportResolver;
        public HomeController(ReportResolver reportResolver) : base(reportResolver.Storage)
        {
            _reportResolver = reportResolver;
        }

        public ActionResult Index()
        {
            var vm = GetReportViewModel();
            return View(vm);
        }

        public ActionResult Report(Guid reportId)
        {
            var vm = GetReportViewModel();
            vm.Report = _reportResolver.GetReport(reportId);
            return View(vm);
        }

        public FileResult ExecuteReport(Guid reportId)
        {
            var vm = GetReportViewModel();

            Report report = _reportResolver.GetReport(reportId);
            report.ReadParameters(Request.QueryString);

            Result result = report.Execute();
            return File(result.AsFile(), result.MimeType, result.FileName);
        }

       
    }
}
