using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReport.Model
{
    public class LinkedReportViewModel : LinkedReport
    {
        public static LinkedReportViewModel FromLinkedRemport(LinkedReport linkedReport)
        {
            var viewModel = new LinkedReportViewModel
            {
                ReportId = linkedReport.ReportId,
                LinkedReportId = linkedReport.LinkedReportId,
                Order = linkedReport.Order
            };
            return viewModel;
        }

        public string Name { get; set; }
    }
}
