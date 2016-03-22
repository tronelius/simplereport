using Newtonsoft.Json;
using SimpleReport.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using SimpleReport.Model.Extensions;
using SimpleReport.Model.Result;
using SimpleReport.Model.Storage;

namespace SimpleReport.ViewModel
{
    public class DesignerViewModel 
    {
        public Report Report { get; set; }
        public IEnumerable<Report> Reports { get; set; }
        public IEnumerable<KeyValue> InputTypes {get;set;}
        public IEnumerable<Connection> Connections { get; set; }
        public IEnumerable<LookupReport> LookupReports { get; set; }
        public List<Access> AccessLists { get; set; }
        public IEnumerable<TypeAheadReport> TypeAheadReports { get; set; }
        public List<Access> ReportOwnerAccessLists { get; set; }
        public Settings Settings { get; set; }
        public IEnumerable<AccessEditorViewModel> AccessEditorViewModel { get; set; }
        public bool SubscriptionEnabled { get; set; }
        public List<ResultInfo> ReportResultTypes { get; set; }

        public DesignerViewModel(IStorage reportStorage, IPrincipal user, IApplicationSettings applicationSettings)
        {
            Reports = reportStorage.GetAllReports();
            Connections = reportStorage.GetConnections();
            LookupReports = reportStorage.GetLookupReports();
            AccessLists = reportStorage.GetAccessLists().ToList();
            TypeAheadReports = reportStorage.GetTypeAheadReports().ToList();
            ReportOwnerAccessLists = reportStorage.GetAccessLists().ToList();
            ParameterInputType types = new ParameterInputType();
            InputTypes = types.ToKeyValues();
            AccessLists.Insert(0,new Access(Guid.Empty,"Free for all",""));
            ReportOwnerAccessLists.Insert(0, new Access(Guid.Empty, "None selected", ""));
            Settings = reportStorage.GetSettings();
            AccessEditorViewModel = Enum.GetValues(typeof(AccessStyle)).Cast<AccessStyle>().Select(x => new AccessEditorViewModel(x, GetTextForTemplateEditor(x)));
            SubscriptionEnabled = applicationSettings.SubscriptionEnabled;
            ReportResultTypes = ResultFactory.GetList();
        }

        

        private string GetTextForTemplateEditor(AccessStyle editor)
        {
            switch (editor)
            {
                case AccessStyle.Administrators:
                    return "Admininstrators";
                case  AccessStyle.Anyone:
                    return "Anyone with report access";
                case AccessStyle.ReportOwner:
                        return "Report owner";
                default:
                    return editor.ToString();
            }    
        }
    }
}