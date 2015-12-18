using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using SimpleReport.Helpers;
using SimpleReport.Model;
using SimpleReport.Model.Exceptions;
using SimpleReport.Model.Helpers;
using SimpleReport.Model.Logging;

namespace SimpleReport.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ReportResolver _reportResolver;

        public HomeController(ReportResolver reportResolver, ILogger logger, IApplicationSettings applicationSettings) : base(reportResolver.Storage, logger, applicationSettings)
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
            report.ReadParameters(Request.QueryString);
            if (!report.IsAvailableForMe(User, _adminAccess))
                ModelState.AddModelError("AccessControl", "You don't have access to view this report!");
            else
                vm.Report = report;

            vm.CanEditTemplate = report.IsAvailableToEditTemplate(User, _adminAccess);
            vm.CanEditSubscriptions = report.IsAvailbleToEditSubscriptions(User, _adminAccess);

            return View(vm);
        }

        public ActionResult ExecuteReport(Guid reportId)
        {
            Report report = _reportResolver.GetReport(reportId);
            if (report.IsAvailableForMe(User, _adminAccess))
            {
                report.ReadParameters(Request.QueryString);

                byte[] templateData = null;
                if (report.HasTemplate)
                    templateData = _reportResolver.Storage.GetTemplate(reportId).Bytes;

                Result result= null;

                if (report.TemplateFormat==TemplateFormat.Excel || report.TemplateFormat==TemplateFormat.Empty)
                    result = report.ExecuteWithTemplate(templateData);
                else if (report.TemplateFormat == TemplateFormat.Word)
                    result = report.ExecuteWithWordTemplate(templateData);

                if (result != null && result.HasData())
                    return File(result.AsFile(), result.MimeType, result.FileName);
                return new HttpStatusCodeResult(204);
            }
            return File(GetBytes("Not allowed to execute this report"), "text/plain", "NotAllowed.txt");
        }

        public ActionResult ExecuteOnScreenReport(Guid reportId)
        {
            Report report = _reportResolver.GetReport(reportId);
            if (report.IsAvailableForMe(User, _adminAccess))
            {
                report.ReadParameters(Request.QueryString);
                
                var data = report.ExecuteAsRawData();
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(new { error = "Not Authorized" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadTemplate(Guid reportId)
        {
            try
            {
                Report report = _reportResolver.GetReport(reportId);
                if (!report.IsAvailableToEditTemplate(User, _adminAccess))
                    return Json(new {error = "Not Authorized"});

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        byte[] data;
                        using (Stream inputStream = file.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }
                            data = memoryStream.ToArray();
                        }

                        var mimeType = MimeMapping.GetMimeMapping(file.FileName);

                        try
                        {
                            if (MimeTypeHelper.IsWord(mimeType))
                            {
                                HandleWord(report, reportId, data);
                            }
                            else
                            {
                                HandleExcel(report, reportId, data);
                            }
                        }
                        catch (ValidationException vex)
                        {
                            return Json(new {error = vex.Message});
                        }
                    }
                }
                return Json(new { status = "ok", TemplateFormat = report.TemplateFormat });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in UploadTemplate", ex);
                return Json(new {error = "Error Uploading Template for Report"});
            }
        }

        public ActionResult DeleteTemplate(Guid reportId)
        {
            Report report = _reportResolver.GetReport(reportId);
            if (!report.IsAvailableToEditTemplate(User, _adminAccess))
                return Json(new {error = "Not Authorized"});
            try
            {
                _reportResolver.Storage.DeleteTemplate(reportId);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in DeleteTemplate", ex);
                return Json(new {error = "Error when deleting the Template"});
            }
            return Json(new {status = "ok"});
        }

        private void HandleExcel(Report report, Guid reportId, byte[] data)
        {
            ExcelValidator.Validate(data);//throws on invalid;

            _reportResolver.Storage.SaveTemplate(data, ".xlsx", reportId);
            report.TemplateFormat = TemplateFormat.Excel;
            _reportResolver.Storage.SaveReport(report);
        }

        private void HandleWord(Report report, Guid reportId, byte[] data)
        {
            _reportResolver.Storage.SaveTemplate(data, ".docx", reportId);
            report.TemplateFormat = TemplateFormat.Word;
            _reportResolver.Storage.SaveReport(report);
        }

        public ActionResult DownloadTemplate(Guid reportId)
        {
            var template = _reportResolver.Storage.GetTemplate(reportId);
            return File(template.Bytes, MimeMapping.GetMimeMapping(template.Filename) , template.Filename);
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
