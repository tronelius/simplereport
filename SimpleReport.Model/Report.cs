using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Web.UI.WebControls;

namespace SimpleReport.Model
{

    public enum AccessStyle
    {
        Administrators,
        ReportOwner,
        Anyone
    }
    
    public class Report : LookupReport
    {
        public ResultType ResultType { get; set; }
        public ParameterList Parameters { get; set; }
        public bool HasTemplate { get; set; }
        public string MailSubject { get; set; }
        public string MailText { get; set; }
        
        //who can fiddle with the template
        public Guid ReportOwnerAccessId { get; set; } 
        [NonSerialized]
        public Access ReportOwnerAccess; //TODO rename to reportowner

        public bool OnScreenFormatAllowed { get; set; }
        public AccessStyle TemplateEditorAccessStyle { get; set; }
        public AccessStyle SubscriptionAccessStyle { get; set; }

        public Report()
        {
            ResultType= ResultType.SimpleExcel;
            Parameters = new ParameterList();
        } 


        public bool IsParameterValueValid()
        {
            return Parameters.All(p => p.IsValid());
        }

        public bool HasMailTemplateChanged(Report reportWithPossibleChanges)
        {
            return !(String.Equals(MailSubject, reportWithPossibleChanges.MailSubject, StringComparison.CurrentCulture) && String.Equals(MailText, reportWithPossibleChanges.MailText, StringComparison.CurrentCulture));
        }

        public bool IsAvailableForMe(IPrincipal user, Access adminAccess)
        {
            return (ReportOwnerAccess != null && ReportOwnerAccess.IsAvailableForMe(user)) ||  (Access == null || Access.IsAvailableForMe(user) || adminAccess.IsAvailableForMe(user));
        }

        public void ReadParameters(NameValueCollection queryString)
        {
            Parameters.ReadParameters(queryString);
        }

        public void IsAllowedToEditTemplate(IPrincipal user, Access adminAccess)
        {
            if (!IsAvailableToEditTemplate(user, adminAccess))
                throw new Exception("Not allowed to edit template in report");
        }

        public bool IsAvailableToEditTemplate(IPrincipal user, Access adminAccess)
        {
            if (TemplateEditorAccessStyle == AccessStyle.Anyone)
                return true;

            if (adminAccess.IsAvailableForMe(user))
                return true;

            if (TemplateEditorAccessStyle == AccessStyle.ReportOwner)
                return ReportOwnerAccess != null && ReportOwnerAccess.IsAvailableForMe(user);

            return false;
        }

        public void IsAllowedToEditSubscriptions(IPrincipal user, Access adminAccess)
        {
            if (!IsAvailbleToEditSubscriptions(user, adminAccess))
                throw new Exception("Not allowed to edit subscriptions for report");
        }

        public bool IsAvailbleToEditSubscriptions(IPrincipal user, Access adminAccess)
        {
            if (SubscriptionAccessStyle == AccessStyle.Anyone)
                return true;

            if (adminAccess.IsAvailableForMe(user))
                return true;

            if (SubscriptionAccessStyle == AccessStyle.ReportOwner)
                return ReportOwnerAccess != null && ReportOwnerAccess.IsAvailableForMe(user);

            return false;
        }

        public RawReportResult ExecuteAsRawData()
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");

            DataTable result = ADO.GetResults(Connection, Sql, Parameters.CreateParameters());

            var raw = new RawReportResult
            {
                Headers = result.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray(),
                Rows = result.Rows.Cast<DataRow>().Select(x => x.ItemArray.Select(Stringify).ToArray()).ToArray()
            };

            return raw;
        }

        public Result ExecuteWithTemplate(byte[] templateData)
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");

            DataTable result = ADO.GetResults(Connection, Sql, Parameters.CreateParameters());
            return new Result(this.ResultType, result, this, templateData);
        }

        private string Stringify(object obj)
        {
            if (obj is DateTime)
                return ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss");

            return obj.ToString();
        }

       
    }
}
