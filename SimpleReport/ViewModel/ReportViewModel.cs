using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleReport.Model;

namespace SimpleReport.ViewModel
{
    public class ReportInfoViewModel
    {
        public IEnumerable<ReportInfo> Reports { get; set; }
    }

    public class ReportViewModel : ReportInfoViewModel
    {
        
        public Report Report { get; set; }
    }
}