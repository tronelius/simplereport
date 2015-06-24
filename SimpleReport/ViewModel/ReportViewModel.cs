using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
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

    public class ReportViewModel : ReportInfoViewModel
    {
        public Report Report { get; set; }
        public bool HasAdminAccess { get; set; }
        public bool CanEditTemplate { get; set; }
        public bool CanEditSubscriptions { get; set; }
        public bool SubscriptionEnabled { get; set; }
    }
}