using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace SimpleReport.Model
{
    public class Result
    {
        public ResultType Type { get; private set; }
        public DataTable Table { get; private set; }
        public Report Report { get; private set; }
        public string FileName { get { return Report.Name + "_created@" + DateTime.Now.ToString() + getFileExtension(); } }
        public string MimeType { get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }}


        private string getFileExtension()
        {
            return ".xlsx";
        }

        public Result(ResultType type, DataTable table, Report report)
        {
            Type = type;
            Table = table;
            Report = report;
        }

        public byte[] AsFile()
        {
            switch (Type)
            {
                case ResultType.SimpleExcel:
                    {
                        using (ExcelPackage pck = new ExcelPackage())
                        {
                            ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Result");
                            ws.Cells["A2"].LoadFromDataTable(Table, true);
                            return pck.GetAsByteArray();
                        }
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException("Unknown ResultType");
            }
            
        }
    }

    public enum ResultType
    {
        SimpleExcel = 0
    }
}