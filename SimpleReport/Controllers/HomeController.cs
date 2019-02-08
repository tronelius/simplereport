using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OpenXmlPowerTools;
using SimpleReport.Helpers;
using SimpleReport.Model;
using SimpleReport.Model.Constants;
using SimpleReport.Model.Exceptions;
using SimpleReport.Model.Helpers;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Result;
using SimpleReport.Model.Service;
using WebGrease.Css.Extensions;

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
                if (report.ReportType == ReportType.MultiReport)
                {
                    return ExecuteMultiReport(report);
                }
                report.ReadParameters(Request.QueryString);

                Template template = null;
                if (report.HasTemplate)
                    template = _reportResolver.Storage.GetTemplate(reportId);

                Report detailReport = null;
                if (report.DetailReportId.HasValue)
                {
                    detailReport = _reportResolver.GetReport(report.DetailReportId.Value);
                }

                ResultFileInfo result = report.ExecuteWithTemplate(template, detailReport);

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

        private ActionResult ExecuteMultiReport(Report report)
        {
            var files = new List<ResultFileInfo>();
            var mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var fileName = GetMultiReportName(report);
            foreach (var linkedReport in report.ReportList.OrderBy(r =>r.Order))
            {
                var rep = _reportResolver.GetReport(linkedReport.LinkedReportId);
                rep.ReadMultiReportParameters(Request.QueryString);

                Template template = null;
                if (report.HasTemplate)
                    template = _reportResolver.Storage.GetTemplate(rep.Id);

                Report detailReport = null;
                if (rep.DetailReportId.HasValue)
                {
                    detailReport = _reportResolver.GetReport(rep.DetailReportId.Value);
                }

                var result = rep.ExecuteWithTemplate(template, detailReport);

                if (result != null)
                {
                    files.Add(result);
                }
            }

            var bytes = new List<byte[]>();
            foreach (var result in files)
            {
                bytes.Add(result.Data);
            }
            
            var combinedDocuments = CombineDocuments(bytes);
            var resultDocument = new ResultFileInfo(fileName, mimeType, combinedDocuments);
            var file = File(combinedDocuments, mimeType, fileName);

            if (combinedDocuments.Length > 0 && report.ConvertToPdf)
            {
                resultDocument = _pdfService.ConvertToPdf(resultDocument);
                file = File(resultDocument.Data, resultDocument.MimeType, resultDocument.FileName);
            }

            return file;
        }

        public string GetMultiReportName(Report report)
        {
            return report.Name + "_created@" + DateTime.Now.ToString(CultureInfo.InvariantCulture) + ".docx";
        }

        public byte[] CombineDocuments(IList<byte[]> documents)
        {
            var sources = new List<Source>();
            foreach (var document in documents)
            {
                using (var stream = new MemoryStream())
                {
                    stream.Write(document, 0, document.Length);
                    var wmlDocument = new WmlDocument("doc", stream);
                    var source = new Source(wmlDocument,true);
                    sources.Add(source);
                }
            }
            var combinedDocument = DocumentBuilder.BuildDocument(sources);
            return combinedDocument.DocumentByteArray;
        }


        public ActionResult ExecuteOnScreenReport(Guid reportId)
        {
            Report report = _reportResolver.GetReport(reportId);
            if (report.IsAvailableForMe(User, _adminAccess))
            {
                report.ReadParameters(Request.QueryString);

                var data = report.ExecuteAsRawData();

                if (report.DetailReportId.HasValue)
                {
                    var detailReport = _reportResolver.GetReport(report.DetailReportId.Value);

                    data.Rows.ForEach(r =>
                    {
                        var url = DetailReportUrlHelper.GetUrl(report, detailReport, data.Headers, r);

                        r[0] = $"<a href=\"{url}\" target=\"_blank\">{r[0]}</a>";
                    });
                }

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
                    return Json(new { error = "Not Authorized" });

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
                            return Json(new { error = vex.Message });
                        }
                    }
                }
                return Json(new { status = "ok", TemplateFormat = report.TemplateFormat, ReportResultType = report.ReportResultType });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in UploadTemplate", ex);
                return Json(new { error = "Error Uploading Template for Report" });
            }
        }

        [HttpPost]
        public ActionResult UpdateTemplateMetadata(Guid reportId, bool convertToPdf, string reportResultType)
        {
            Report report = _reportResolver.GetReport(reportId);
            if (report.IsAvailableForMe(User, _adminAccess))
            {
                report.ConvertToPdf = convertToPdf;
                if (ResultFactory.GetList().Exists(a => a.ReportResultType == reportResultType))
                    report.ReportResultType = reportResultType;

                _reportResolver.Storage.SaveReport(report);
                return new HttpStatusCodeResult(200);
            }
            return File(GetBytes("Not allowed to execute this report"), "text/plain", "NotAllowed.txt");
        }

        public ActionResult DeleteTemplate(Guid reportId)
        {
            Report report = _reportResolver.GetReport(reportId);
            if (!report.IsAvailableToEditTemplate(User, _adminAccess))
                return Json(new { error = "Not Authorized" });
            try
            {
                _reportResolver.Storage.DeleteTemplate(reportId);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in DeleteTemplate", ex);
                return Json(new { error = "Error when deleting the Template" });
            }
            return Json(new { status = "ok" });
        }

        public ActionResult GetTypeAheadData(Guid reportId, Guid typeaheadid, string search)
        {
            Report report = _reportResolver.GetReport(reportId);
            if (!report.IsAvailableForMe(User, _adminAccess))
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
            report.ReportResultType = ResultFactory.GetNextResult(report, new Template() { Bytes = data, Mime = MimeTypeHelper.WordMime, TemplateFormat = TemplateFormat.Word }).ResultInfo.ReportResultType;
            _reportResolver.Storage.SaveReport(report);
        }

        public ActionResult DownloadTemplate(Guid reportId)
        {
            var template = _reportResolver.Storage.GetTemplate(reportId);
            if (template != null)
                return File(template.Bytes, MimeMapping.GetMimeMapping(template.Filename), template.Filename);
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
