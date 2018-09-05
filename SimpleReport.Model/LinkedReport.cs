using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReport.Model
{
    public class LinkedReport
    {
        public Guid ReportId { get; set; }
        public Guid LinkedReportId { get; set; }
        public int Order { get; set; }
    }
}
