using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using SimpleReport.Model.DbExecutor;

namespace SimpleReport.Model
{
    public class LookupReport : ReportInfo
    {
        [NonSerialized]
        public Connection Connection;
        [NonSerialized]
        public int SqlTimeout = 60;
        public Guid? ConnectionId { get; set; }
        [Required]
        public string Sql { get; set; }

        public LookupReport()
        {
        }


        public Dictionary<string, string> Execute()
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");

            DataTable result = DbExecutorFactory.GetInstance(Connection).GetResults(Connection, Sql, null, SqlTimeout);
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