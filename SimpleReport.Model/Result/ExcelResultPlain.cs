using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using SimpleReport.Model.Exceptions;
using SimpleReport.Model.Helpers;

namespace SimpleReport.Model.Result
{
    public class ExcelResultPlain : Result
    {
        protected override string getMimeType()
        {
            return MimeTypeHelper.ExcelMime;
        }

        protected override string getFileExtension()
        {
            return ".xlsx";
        }

        public static IEnumerable<ValidationResult> Validate(Report report, Template template)
        {
            using (MemoryStream memStream = new MemoryStream(template.Bytes))
            using (ExcelPackage pck = new ExcelPackage(memStream))
            {
                if (!pck.Workbook.Worksheets.Any(x => x.Name == "Data"))
                    yield return new ValidationResult("The uploaded template must contain an empty sheet named 'Data'");

                var sheet = pck.Workbook.Worksheets["Data"];
                if (sheet.Dimension != null)
                {
                    for (var i = 1; i <= sheet.Dimension.Rows; i++)
                    {
                        if (sheet.Cells["A" + i].Value != null)
                            yield return new ValidationResult("The uploaded templates data-sheet is not empty");
                    }
                }
            }
        }

        public override ResultFileInfo Render(List<DataTable> tables)
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
                ws.Cells["A1"].LoadFromDataTable(tables.First(), true);

                ws.Workbook.Calculate();
                return new ResultFileInfo(this.FileName, this.MimeType, pck.GetAsByteArray());
            }
        }

        public override ResultInfo ResultInfo { get { return new ResultInfo("ExcelResultPlain", "Excel");} }

        public ExcelResultPlain()
        {
            
        }
        public ExcelResultPlain (Report report, Template template) :base(report, template) { }
}
}