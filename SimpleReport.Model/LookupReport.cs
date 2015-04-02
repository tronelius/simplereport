using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace SimpleReport.Model
{
    public class LookupReport : ReportInfo
    {
        [NonSerialized]
        public Connection Connection;
        [Required]
        public Guid ConnectionId { get; set; }
        [Required]
        public string Sql { get; set; }

        public LookupReport()
        {
        }

        public LookupReport(Guid id, string name, string description, Guid connectionId,string sql, string group) : base(id, name, description, group)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new Exception(string.Format("Missing SQL in report with Name:{0}", name));
            ConnectionId = connectionId;
            Sql = sql;
        }

        public Dictionary<string, string> Execute()
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");

            DataTable result = ADO.GetResults<dynamic>(Connection, Sql,null);
            Dictionary<string, string> collection = new Dictionary<string, string>();
            if (result.Columns.Contains("id") && result.Columns.Contains("name"))
            {
                foreach (DataRow row in result.Rows)
                {
                    collection.Add(row["id"].ToString(), row["name"].ToString());
                }
            }
            else if (result.Columns.Count >=2)
            {
                foreach (DataRow row in result.Rows)
                {
                    collection.Add(row[0].ToString(), row[1].ToString());
                }
            }
            return collection;
        }
    }
}