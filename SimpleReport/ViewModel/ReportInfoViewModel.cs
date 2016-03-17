using System.Collections.Generic;
using System.Linq;
using SimpleReport.Model;

namespace SimpleReport.ViewModel
{
    public class ReportInfoViewModel
    {
        public ILookup<string, ReportInfo> ReportGroups
        {
            get
            {
                return Reports.ToLookup(r => r.Group);
            }
        } 
        public IEnumerable<ReportInfo> Reports { get; set; }

    }
}