using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperExtensions;
using SimpleReport.Model.Storage.SQL;

namespace SimpleReport.Model
{
    public static class ADO
    {
        public static DataTable GetResults(Connection conn, string query, IEnumerable<SqlParameter> param)
        {
            using (SqlConnection cn = GetOpenConnection(conn))
            {
                try
                {
                    SqlCommand cmd = null;
                    DataTable table = new DataTable();
                    cmd = CreateCommand(cn);
                    cmd.CommandType = query.ToLower().StartsWith("select ") ? CommandType.Text : CommandType.StoredProcedure;
                    cmd.CommandText = query;
                    if (param != null)
                        cmd.Parameters.AddRange(param.ToArray());

                    SqlDataAdapter da = null;
                    using (da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(table);
                    }

                    return table;


                }
                finally
                {
                    cn.Close();
                }
            }
        }

        public static IEnumerable<IdName> getResultsAsIdName(Connection conn, string query, IEnumerable<SqlParameter> param)
        {
            using (SqlConnection cn = GetOpenConnection(conn))
            {
                return cn.Query<IdName>(query, param);
            }
        }

        public static IEnumerable<dynamic> getResultsAsDynamic(Connection conn, string query, IEnumerable<SqlParameter> param)
        {
            using (SqlConnection cn = GetOpenConnection(conn))
            {
                return cn.Query(query, param);
            }
        }

        public static IEnumerable<IEnumerable<dynamic>> GetMultipleResultsDynamic(Connection conn, string query, IEnumerable<SqlParameter> param)
        {
            using (SqlConnection cn = GetOpenConnection(conn))
            {
                try
                {
                    var args = new DynamicParameters(new { });
                    param.ToList().ForEach(p => args.Add(p.ParameterName, p.Value));
                    using (var gridReader = cn.QueryMultiple(query, args))
                    {
                        do
                        {
                            yield return gridReader.Read();
                        } while (!gridReader.IsConsumed);
                    }
                    
                }
                finally
                {
                    cn.Close();
                }
            }
        }


        public static List<DataTable> GetMultipleResults(Connection conn, string query, IEnumerable<SqlParameter> param)
        {
            var tables = new List<DataTable>();
            using (SqlConnection cn = GetOpenConnection(conn))
            {
            try
            {
                SqlCommand cmd = null;
                cmd = CreateCommand(cn);
                cmd.CommandType = query.ToLower().StartsWith("select ") ? CommandType.Text : CommandType.StoredProcedure;
                cmd.CommandText = query;
                if (param != null)
                    cmd.Parameters.AddRange(param.ToArray());

                    var reader = cmd.ExecuteReader();

                    //havnt got a clue on how to get multiple datatables in a better way..
                    while (reader.HasRows)
                    {
                        var data = GetDataTable(reader);

                        while (reader.Read())
                        {
                            AddRowWithData(data, reader);
                        }

                        tables.Add(data);
                        reader.NextResult();
                    }
            }
            catch (Exception)
            {
                cn.Close();
                throw;
            }
            }
            return tables;
        }

        private static void AddRowWithData(DataTable data, SqlDataReader reader)
        {
            var newRow = data.Rows.Add();

            for (int i = 0; i < data.Columns.Count; i++)
            {
                newRow[i] = reader[i];
            }
        }

        private static DataTable GetDataTable(SqlDataReader reader)
        {
            DataTable schemaTable = reader.GetSchemaTable();
            DataTable data = new DataTable();
            var takenColumns = new Dictionary<string, int>();
            foreach (DataRow row in schemaTable.Rows)
            {
                string colName = row.Field<string>("ColumnName");

                //append a number to the column in case there are duplicates. The table gets sad otherwise.
                if (!takenColumns.ContainsKey(colName))
                    takenColumns.Add(colName, 0);
                else
                {
                    takenColumns[colName]++;
                    colName = colName + takenColumns[colName];
                }

                Type t = row.Field<Type>("DataType");
                data.Columns.Add(colName, t);
            }
            return data;
        }

        private static SqlCommand CreateCommand(SqlConnection cn)
        {
            var cmd= cn.CreateCommand();
            cmd.CommandTimeout = 60;
            return cmd;
        }

        private static SqlConnection GetOpenConnection(Connection conn)
        {
            SqlConnection cn;
            try
            {
                cn = new SqlConnection(conn.ConnectionString);
                cn.Open();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error when opening connection to Database, Name:{0}, Connectionstring:{1}", conn.Name, conn.ConnectionString), ex);
            }
            return cn;
        }
    }
}
