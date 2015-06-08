using System;
using System.Linq.Expressions;
using System.Security;
using System.Web.Http.Results;
using System.Web.Mvc;
using SimpleReport.Helpers;
using SimpleReport.Model;
using SimpleReport.Model.Logging;

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

                byte[] templateData = null;
                if (report.HasTemplate)
                    templateData = _reportResolver.Storage.GetTemplate(reportId).Bytes;

                Result result = report.ExecuteWithTemplate(templateData);
                return File(result.AsFile(), result.MimeType, result.FileName);
            }
            return File(GetBytes("Not allowed to execute this report"), "text/plain","NotAllowed.txt");
        }

        public ActionResult UploadTemplate(Guid reportId)
        {
            Report report = _reportResolver.GetReport(reportId);
            if (!report.IsAvailableForMe(User, _adminAccess))
                return Json(new { error = "Not Authorized" });
            
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    _reportResolver.Storage.SaveTemplate(file, reportId);

                    if (!ExcelValidator.Validate(_reportResolver.Storage.GetTemplate(reportId)))
                    {
                        _reportResolver.Storage.DeleteTemplate(reportId);
                        return Json(new {error = "The template must have a tab called Data"});
                    }

                    report.HasTemplate = true;
                    _reportResolver.Storage.SaveReport(report);
                }
            }

            return Json(new {status = "ok"});
        }

        public ActionResult DownloadTemplate(Guid reportId)
        {
            var template = _reportResolver.Storage.GetTemplate(reportId);
            return File(template.Bytes, template.Mime, template.Filename);
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
