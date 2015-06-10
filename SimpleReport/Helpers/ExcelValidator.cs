using System.IO;
using System.Linq;
using OfficeOpenXml;
using SimpleReport.Model.Exceptions;

namespace SimpleReport.Helpers
{
    public static class ExcelValidator
    {
        public static bool Validate(byte[] data)
        {
            using (MemoryStream memStream = new MemoryStream(data))
            using (ExcelPackage pck = new ExcelPackage(memStream))
            {
                if (!pck.Workbook.Worksheets.Any(x => x.Name == "Data"))
                    throw new ValidationException("The uploaded template must contain an empty sheet named 'Data'");
                
                var sheet = pck.Workbook.Worksheets["Data"];
                if (sheet.Dimension != null)
                {
                    for (var i = 1; i <= sheet.Dimension.Rows; i++)
                    {
                        if (sheet.Cells["A" + i].Value != null)
                            throw new ValidationException("The uploaded templates data-sheet is not empty");
                    }
                }

                return true;
            }
        }
    }
}