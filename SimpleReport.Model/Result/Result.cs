using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;

namespace SimpleReport.Model.Result
{
    public abstract class Result
    {
        protected readonly IDataReader _dataReader;
        //protected DataTable Table { get; set; }
        protected Report Report { get; set; }
        public byte[] TemplateData { get; set; }
        public string FileName { get { return HttpUtility.UrlEncode(Report.Name + "_created@" + DateTime.Now.ToString(CultureInfo.InvariantCulture) + getFileExtension()); } }
        public string MimeType { get { return getMimeType(); } }
      
        protected abstract string getMimeType();
        
        protected abstract string getFileExtension();

        public abstract ResultFileInfo Render(List<DataTable> tables);

        public abstract ResultInfo ResultInfo { get; }

        public Result()
        {
            
        }
        public Result(Report report, Template template)
        {
            Report = report;
            if (template != null)
                TemplateData = template.Bytes;
        }
        
    }

    public static class FieldHandles
    {
        public static string Merge = "merge_id";
        public static string from = "@from";
        public static string to = "@to";
        public static string table = "table";
    }

}