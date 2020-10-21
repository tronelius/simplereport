using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using SimpleReport.Model;
using SimpleReport.Model.Result;

namespace SimpleReport.ViewModel
{
    public class ReportViewModel : ReportInfoViewModel
    {
        public Report Report { get; set; }
        public bool HasAdminAccess { get; set; }
        public bool HasSubscriptionAccess { get; set; }
        public bool CanEditTemplate { get; set; }
        public bool CanEditSubscriptions { get; set; }
        public bool SubscriptionEnabled { get; set; }

        public List<ResultInfo> ReportResultTypes { get; set; }
    }
}