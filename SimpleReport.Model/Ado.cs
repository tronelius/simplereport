using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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
                        DataTable schemaTable = reader.GetSchemaTable();
                        DataTable data = new DataTable();
                        var takenColumns = new Dictionary<string,int>();
                        foreach (DataRow row in schemaTable.Rows)
                        {
                            string colName = row.Field<string>("ColumnName");

                            //append a number to the column in case there are duplications
                            if(!takenColumns.ContainsKey(colName))
                                takenColumns.Add(colName, 0);
                            else
                            {
                                takenColumns[colName]++;
                                colName = colName + takenColumns[colName];
                            }

                            Type t = row.Field<Type>("DataType");
                            data.Columns.Add(colName, t);
                            
                        }
                        while (reader.Read())
                        {
                            var newRow = data.Rows.Add();

                            for (int i = 0; i < data.Columns.Count; i++)
                            {
                                newRow[i] = reader[i];
                            }
                        }

                        tables.Add(data);
                        reader.NextResult();
                    }
                }
                catch (Exception ex)
                {
                    cn.Close();
                    throw;
                }
            }
            return tables;
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
                throw new Exception(
                    String.Format("Error when opening connection to Database, Name:{0}, Connectionstring:{1}", conn.Name,
                                  conn.ConnectionString), ex);
            }
            return cn;
        }
    }
}
