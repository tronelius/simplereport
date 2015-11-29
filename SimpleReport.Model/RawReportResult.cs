using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleReport.Model
{
    public class RawReportResult
    {
        public string[] Headers { get; set; }
        public string[][] Rows { get; set; }
    }
}
