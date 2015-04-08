using Newtonsoft.Json;
using SimpleReport.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using SimpleReport.Model.Extensions;
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
        public Settings Settings { get; set; }

        public DesignerViewModel(IStorage reportStorage, IPrincipal user)
        {
            Reports = reportStorage.GetAllReports();
            Connections = reportStorage.GetConnections();
            LookupReports = reportStorage.GetLookupReports();
            AccessLists = reportStorage.GetAccessLists().ToList();
            ParameterInputType types = new ParameterInputType();
            InputTypes = types.ToKeyValues();
            AccessLists.Insert(0,new Access(Guid.Empty,"Free for all",""));
            Settings = reportStorage.GetSettings();
        }
    }
}