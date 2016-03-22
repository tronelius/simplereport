using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SimpleReport.Model
{
    public class TypeAheadReport : LookupReport
    {

        public Dictionary<string, string> Execute(string searchword)
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");
            List<SqlParameter> paramList = new List<SqlParameter>();
            SqlParameter param = new SqlParameter("@search",SqlDbType.NVarChar,100);
            param.Value = searchword;
            paramList.Add(param);

            DataTable result = ADO.GetResults(Connection, Sql, paramList);

            Dictionary<string, string> collection = new Dictionary<string, string>();
            if (result.Columns.Contains("id") && result.Columns.Contains("name"))
            {
                foreach (DataRow row in result.Rows)
                {
                    collection.Add(row["id"].ToString(), row["name"].ToString());
                }
            }
            else if (result.Columns.Count >= 2)
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