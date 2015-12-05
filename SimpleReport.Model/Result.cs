using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace SimpleReport.Model
{
    public class Result
    {
        private readonly IDataReader _dataReader;
        private ResultType Type { get; set; }
        private DataTable Table { get; set; }
        private Report Report { get; set; }
        private byte[] TemplateData { get; set; }
        public string FileName { get { return Report.Name + "_created@" + DateTime.Now.ToString() + getFileExtension(); } }
        public string MimeType { get { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }}


        private string getFileExtension()
        {
            return ".xlsx";
        }

        public bool HasData()
        {
            return Table.Rows.Count > 0;
        }

        public Result(ResultType type, DataTable table, Report report, byte[] templateData)
        {
            Type = type;
            Table = table;
            Report = report;
            TemplateData = templateData;
        }

        public Result(ResultType type, IDataReader dataReader, Report report)
        {
            _dataReader = dataReader;
            Type = type;
            Report = report;
        }


        //public Stream AsExcelFileStream(IDataReader reportData, ExcelWorksheet ws)
        //{
            
        //}

        //private void PopulateExcelWithInsertedRows(IDataReader reportData, ExcelWorksheet ws)
        //{
        //    int row = 2;
        //    ws.Cells[2, 1, reportData.Count, statisticsReportDefinitionMetric.Count].Clear(); //Clear any existing data in template, but not custom added formula in new columns.

        //    //rows
        //    for (int i = 0; i < reportData.Count; i++)
        //    {
        //        IDictionary<string, object> rowItem = reportData[i] as IDictionary<string, object>;
        //        ws.Cells[2, 1, 2, 100].Copy(ws.Cells[row, 1, row, 100]); //copy row from second row to get the custom formulas copied.

        //        //columns
        //        ws.Cells[row, 1].Value = rowItem[_periodColumn];
        //        for (int columnindex = 0; columnindex < statisticsReportDefinitionMetric.Count; columnindex++)
        //        {
        //            ws.Cells[row, columnindex + 2].Value = statisticsReportDefinitionMetric[columnindex].TransformToString(rowItem[statisticsReportDefinitionMetric[columnindex].GetColumnName()], true);
        //        }
        //        row++;
        //    }
        //}

        public byte[] AsFile()
        {
            switch (Type)
            {
                case ResultType.SimpleExcel:
                    {
                        using (ExcelPackage pck = new ExcelPackage())
                        {
                            ExcelWorksheet ws;

                            if (TemplateData != null)
                            {
                                using (MemoryStream memStream = new MemoryStream(TemplateData))
                                {
                                    pck.Load(memStream);
                                }

                                ws = pck.Workbook.Worksheets["Data"];
                            }
                            else
                                ws = pck.Workbook.Worksheets.Add("Data");

                            if (_dataReader != null)
                            {
                                
                                //ws.Cells["A2"].LoadFromDataReader(_dataReader, true);
                            }
                            else
                                ws.Cells["A1"].LoadFromDataTable(Table, true);

                            ws.Workbook.Calculate();
                            return pck.GetAsByteArray();
                        }
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