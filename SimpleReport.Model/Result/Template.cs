using System;
using System.IO;
using System.Security.RightsManagement;
using DocumentFormat.OpenXml.Packaging;
using SimpleReport.Model.Constants;
using SimpleReport.Model.Helpers;

namespace SimpleReport.Model.Result
{
    public class Template
    {
        public byte[] Bytes { get; set; }
        public string Mime { get; set; }
        public string Filename { get; set; }
        public TemplateFormat TemplateFormat { get; set; }
    }

    public enum ReportStyle
    {
        SimpleTableReport = 1,
        MasterDetailReport =2
    }

    //public class TemplateDetector
    //{

    //    public TemplateInfo DetectTemplate(string mimeType, byte[] data)
    //    {
    //        TemplateInfo tInfo = new TemplateInfo();
    //        if (MimeTypeHelper.IsWord(mimeType)) { 
    //            tInfo.TemplateFormat = TemplateFormat.Word;
    //            tInfo.TemplateStyle = DetectStyle(data);
    //        }
    //        else 
    //            tInfo.TemplateFormat = TemplateFormat.Excel;

    //        return tInfo;
    //    }

    //    private TemplateInfo DetectStyle(byte[] data)
    //    {
    //        using (var tdata = new MemoryStream(data))
    //        using (var ms = new MemoryStream())
    //        {
    //            tdata.CopyTo(ms);
    //            ms.Position = 0;
    //            using (WordprocessingDocument doc = WordprocessingDocument.Open(ms, true))
    //            {

    //            }
    //        }
    //    }
            
    //}

    public class TemplateInfo
    {
        public TemplateFormat TemplateFormat { get; set; }
        public ReportStyle ReportStyle { get; set; }
    }
}
