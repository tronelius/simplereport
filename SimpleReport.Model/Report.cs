using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using SimpleReport.Model.DbExecutor;
using SimpleReport.Model.Helpers;
using SimpleReport.Model.Result;

namespace SimpleReport.Model
{

    public enum AccessStyle
    {
        Administrators,
        ReportOwner,
        Anyone
    }

    public enum TemplateFormat
    {
        Empty=0,
        Excel=1,
        Word=2
    }
    
    public class Report : LookupReport
    {
        public ParameterList Parameters { get; set; }
        public bool HasTemplate
        {
            get { return TemplateFormat != TemplateFormat.Empty; }
        }
        public Guid? DetailReportId { get; set; }
        public string MailSubject { get; set; }
        public string MailText { get; set; }
        public bool OnScreenFormatAllowed { get; set; }
        public AccessStyle TemplateEditorAccessStyle { get; set; }
        public AccessStyle SubscriptionAccessStyle { get; set; }
        public TemplateFormat TemplateFormat { get; set; }

        public string ReportResultType { get; set; }
        public bool ConvertToPdf { get; set; }

        public Report()
        {
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

            var db = DbExecutorFactory.GetInstance(Connection);
            var parameters = Parameters.CreateParameters(Sql, UpdateSql, db);
            DataTable result = db.GetResults(Connection, Sql, parameters);

            var raw = new RawReportResult
            {
                Headers = result.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray(),
                Rows = result.Rows.Cast<DataRow>().Select(x => x.ItemArray.Select(Stringify).ToArray()).ToArray()
            };

            return raw;
        }

        public ResultFileInfo ExecuteWithTemplate(Template template, Report detailReport = null)
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");

            var db = DbExecutorFactory.GetInstance(Connection);
            var parameters = Parameters.CreateParameters(Sql, UpdateSql, db);
            var dataResult = db.GetMultipleResults(Connection, Sql, parameters);

            if (dataResult.Count == 0)
                return null;

            if (detailReport != null)
            {
                var table = dataResult.First();
                table.Columns.Add(new DataColumn("detailurl"));
                var headers = table.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToArray();
                foreach (DataRow tableRow in table.Rows)
                {
                    tableRow["detailurl"] = DetailReportUrlHelper.GetUrl(this, detailReport, headers, tableRow.ItemArray.Select(x => x.ToString()).ToArray());
                }
            }

            var result = ResultFactory.GetInstance(this, template);

            return result.Render(dataResult);
        }

        public void UpdateSql(string sql)
        {
            //some parameters need to modify the sql to work, like when we have an in-clause and need to replace one param with many.
            Sql = sql;
        }

        private string Stringify(object obj)
        {
            if (obj is DateTime)
                return ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss");

            return obj.ToString();
        }

        public bool IsMasterDetailReport()
        {
            string pattern = @"(?<!\()select";
            var result = Regex.Matches(this.Sql, pattern);
            string mergeIdPattern = @"merge_id";
            var mergeResult = Regex.Matches(this.Sql, mergeIdPattern);
            return result.Count > 1 && mergeResult.Count >= 2;
        }
    }
}
