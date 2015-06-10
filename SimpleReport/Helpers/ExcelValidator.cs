using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using SimpleReport.Model;

namespace SimpleReport.Helpers
{
    public static class ExcelValidator
    {
        public static bool Validate(Template template)
        {
            using (MemoryStream memStream = new MemoryStream(template.Bytes))
            using (ExcelPackage pck = new ExcelPackage(memStream))
            {
                if (!pck.Workbook.Worksheets.Any(x => x.Name == "Data"))
                    return false;
                
                var sheet = pck.Workbook.Worksheets["Data"];
                if (sheet.Dimension != null)
                {
                    for (var i = 1; i <= sheet.Dimension.Rows; i++)
                    {
                        if (sheet.Cells["A" + i].Value != null)
                            return false;
                    }
                }

                return true;
            }
        }
    }
}