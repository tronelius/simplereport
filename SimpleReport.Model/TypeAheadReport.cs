using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using SimpleReport.Model.DbExecutor;

namespace SimpleReport.Model
{
    public class TypeAheadReport : LookupReport
    {
        public IEnumerable<IdName> Execute(string searchword)
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");
            var db = DbExecutorFactory.GetInstance(Connection);

            List<DbParameter> paramList = new List<DbParameter>();

            var param = db.CreateStringParameter("Search", 100);
            param.Value = searchword;
            paramList.Add(param);

            DataTable result = db.GetResults(Connection, Sql, paramList);
            List<IdName> collection = new List<IdName>();
            if (result.Columns.Contains("id") && result.Columns.Contains("name"))
            {
                foreach (DataRow row in result.Rows)
                {
                    collection.Add(new IdName() { Id = row["id"].ToString(), Name=row["name"].ToString() });
                }
            }
            else if (result.Columns.Count >= 2)
            {
                foreach (DataRow row in result.Rows)
                {
                    collection.Add(new IdName() {Id=row[0].ToString(),Name= row[1].ToString()});
                }
            }
           return collection;
        }
    }
}