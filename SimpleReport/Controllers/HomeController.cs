using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using SimpleReport.Model;
using SimpleReport.Model.Logging;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ReportResolver _reportResolver;
        public HomeController(ReportResolver reportResolver, ILogger logger) : base(reportResolver.Storage, logger)
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
            var report = _reportResolver.GetReport(reportId);
            if (!report.IsAvailableForMe(User, _adminAccess))
                ModelState.AddModelError("AccessControl","You don't have access to view this report!");
            else
                vm.Report = report;
            return View(vm);
        }

        public FileResult ExecuteReport(Guid reportId)
        {
            
            Report report = _reportResolver.GetReport(reportId);
            if (report.IsAvailableForMe(User, _adminAccess)) { 
                report.ReadParameters(Request.QueryString);

                Result result = report.Execute();
                return File(result.AsFile(), result.MimeType, result.FileName);
            }
            return File(GetBytes("Not allowed to execute this report"), "text/plain","NotAllowed.txt");
        }


        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
