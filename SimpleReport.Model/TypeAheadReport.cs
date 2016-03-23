﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SimpleReport.Model
{
    public class TypeAheadReport : LookupReport
    {

        public IEnumerable<IdName> Execute(string searchword)
        {
            if (Connection == null)
                throw new Exception("Missing Connection in report");
            List<SqlParameter> paramList = new List<SqlParameter>();
            SqlParameter param = new SqlParameter("@search",SqlDbType.NVarChar,100);
            param.Value = searchword;
            paramList.Add(param);

            DataTable result = ADO.GetResults(Connection, Sql, paramList);
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