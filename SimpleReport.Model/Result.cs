using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using OfficeOpenXml.Table;

namespace SimpleReport.Model
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
            return Table.Rows.Count > 0;
        }

        public Result(DataTable table, Report report, byte[] templateData)
        {
            Table = table;
            Report = report;
            TemplateData = templateData;
        }

        public abstract byte[] AsFile();
    }
    
}