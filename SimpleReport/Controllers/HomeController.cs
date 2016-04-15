﻿using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using SimpleReport.Helpers;
using SimpleReport.Model;
using SimpleReport.Model.Exceptions;
using SimpleReport.Model.Helpers;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Result;
using SimpleReport.Model.Service;

namespace SimpleReport.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ReportResolver _reportResolver;
        private readonly IPdfService _pdfService;

        public HomeController(ReportResolver reportResolver, ILogger logger, IApplicationSettings applicationSettings, IPdfService pdfService) : base(reportResolver.Storage, logger, applicationSettings)
        {
            _reportResolver = reportResolver;
            _pdfService = pdfService;
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

                Template template = null;
                if (report.HasTemplate)
                    template = _reportResolver.Storage.GetTemplate(reportId);

                ResultFileInfo result = report.ExecuteWithTemplate(template);

                if (template != null && report.ConvertToPdf)
                {
                    result = _pdfService.ConvertToPdf(result);
                }
                
                if (result != null)
                    return File(result.Data, result.MimeType, result.FileName);
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
                return Json(new { status = "ok", TemplateFormat = report.TemplateFormat, ReportResultType=report.ReportResultType });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in UploadTemplate", ex);
                return Json(new {error = "Error Uploading Template for Report"});
            }
        }

        [HttpPost]
        public ActionResult UpdateTemplateMetadata(Guid reportId, bool convertToPdf)
        {
            Report report = _reportResolver.GetReport(reportId);
            if (report.IsAvailableForMe(User, _adminAccess))
            {
                report.ConvertToPdf = convertToPdf;

                _reportResolver.Storage.SaveReport(report);

                return new HttpStatusCodeResult(200);
            }
            return File(GetBytes("Not allowed to execute this report"), "text/plain", "NotAllowed.txt");
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

        public ActionResult GetTypeAheadData(Guid reportId, Guid typeaheadid, string search)
        {
            Report report = _reportResolver.GetReport(reportId);
            if (!report.IsAvailableToEditTemplate(User, _adminAccess))
                return Json(new { error = "Not Authorized" });
            try
            {
                var typeahead = _reportResolver.Storage.GetTypeAheadReport(typeaheadid);
                return Json(typeahead.Execute(search));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in DeleteTemplate", ex);
                return Json(new { error = "Error when deleting the Template" });
            }
            
        }

        private void HandleExcel(Report report, Guid reportId, byte[] data)
        {
            //ExcelValidator.Validate(data);//throws on invalid;

            _reportResolver.Storage.SaveTemplate(data, ".xlsx", reportId);
            report.TemplateFormat = TemplateFormat.Excel;
            report.ReportResultType = ResultFactory.GetNextResult(report, new Template() { Bytes = data, Mime = MimeTypeHelper.ExcelMime, TemplateFormat = TemplateFormat.Excel }).ResultInfo.ReportResultType;
            _reportResolver.Storage.SaveReport(report);
        }

        private void HandleWord(Report report, Guid reportId, byte[] data)
        {
            _reportResolver.Storage.SaveTemplate(data, ".docx", reportId);
            report.TemplateFormat = TemplateFormat.Word;
            report.ReportResultType = ResultFactory.GetNextResult(report, new Template() {Bytes=data,Mime= MimeTypeHelper.WordMime,TemplateFormat=TemplateFormat.Word}).ResultInfo.ReportResultType;
            _reportResolver.Storage.SaveReport(report);
        }

        public ActionResult DownloadTemplate(Guid reportId)
        {
            var template = _reportResolver.Storage.GetTemplate(reportId);
            if (template != null)
                return File(template.Bytes, MimeMapping.GetMimeMapping(template.Filename) , template.Filename);
            return null;
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
