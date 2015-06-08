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
        public List<Access> TemplateAccessLists { get; set; }
        public Settings Settings { get; set; }
        public IEnumerable<TemplateEditorViewModel> PotentialTemplateEditors { get; set; }

        public DesignerViewModel(IStorage reportStorage, IPrincipal user)
        {
            Reports = reportStorage.GetAllReports();
            Connections = reportStorage.GetConnections();
            LookupReports = reportStorage.GetLookupReports();
            AccessLists = reportStorage.GetAccessLists().ToList();
            TemplateAccessLists = reportStorage.GetAccessLists().ToList();
            ParameterInputType types = new ParameterInputType();
            InputTypes = types.ToKeyValues();
            AccessLists.Insert(0,new Access(Guid.Empty,"Free for all",""));
            TemplateAccessLists.Insert(0, new Access(Guid.Empty, "None selected", ""));
            Settings = reportStorage.GetSettings();
            PotentialTemplateEditors = Enum.GetValues(typeof(TemplateEditorAccessStyle)).Cast<TemplateEditorAccessStyle>().Select(x => new TemplateEditorViewModel(x, GetTextForTemplateEditor(x)));
        }

        private string GetTextForTemplateEditor(TemplateEditorAccessStyle editor)
        {
            switch (editor)
            {
                    case TemplateEditorAccessStyle.Administrators:
                        return "Admininstrators";
                    case  TemplateEditorAccessStyle.Anyone:
                        return "Anyone with access to the report";
                case TemplateEditorAccessStyle.ReportOwner:
                        return "Report owner";
                default:
                    return editor.ToString();
            }    
        }
    }

    public class TemplateEditorViewModel
    {
        public TemplateEditorAccessStyle Value { get; set; }
        public string Text { get; set; }

        public TemplateEditorViewModel(TemplateEditorAccessStyle value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}