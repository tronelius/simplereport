using System.Data;
using System.IO;
using OfficeOpenXml;
using SimpleReport.Model.Helpers;

namespace SimpleReport.Model
{
    public class ExcelResult : Result
    {
        protected override string getMimeType()
        {
            return MimeTypeHelper.ExcelMime;
        }

        protected override string getFileExtension()
        {
            return ".xlsx";
        }


        public override byte[] AsFile()
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

        public ExcelResult(DataTable table, Report report, byte[] templateData) : base(table, report, templateData)
        {
        }

        public ExcelResult(IDataReader dataReader, Report report) : base(dataReader, report)
        {
        }
    }
}