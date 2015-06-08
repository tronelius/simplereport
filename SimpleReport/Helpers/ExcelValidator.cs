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
                return pck.Workbook.Worksheets.Any(x => x.Name == "Data");
            }
        }
    }
}