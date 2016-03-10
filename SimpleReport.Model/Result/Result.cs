using System;
using System.Data;
using System.Globalization;

namespace SimpleReport.Model.Result
{
    public abstract class Result
    {
        protected readonly IDataReader _dataReader;
        protected DataTable Table { get; set; }
        protected Report Report { get; set; }
        protected byte[] TemplateData { get; set; }
        public string FileName { get { return Report.Name + "_created@" + DateTime.Now.ToString(CultureInfo.InvariantCulture) + getFileExtension(); } }
        public string MimeType { get { return getMimeType(); } }

        protected abstract string getMimeType();
        
        protected abstract string getFileExtension();

        public bool HasData()
        {
            return Table?.Rows.Count > 0;
        }

        public Result(){}

        public Result(DataTable table, Report report, byte[] templateData)
        {
            Table = table;
            Report = report;
            TemplateData = templateData;
        }

        public abstract byte[] AsFile();

        
    }

    public static class FieldHandles
    {
        public static string Merge = "merge_id";
        public static string from = "@from";
        public static string to = "@to";
        public static string table = "table";
        public static string tableshort = "TBL";
    }

}