using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using SimpleReport.Model.Replacers;

namespace SimpleReport.Model.DbExecutor
{
    public abstract class BaseExecutor
    {
        private IReplacer _replacer;// = new OracleReservedWordsReplacer();
        public BaseExecutor(IReplacer replacer)
        {
            _replacer = replacer;
        }

        private IEnumerable<DbParameter> ParseParameters(IEnumerable<DbParameter> paramList)
        {
            if (paramList != null) { 
                foreach (var dbParameter in paramList)
                {
                    dbParameter.ParameterName = _replacer.Replace(dbParameter.ParameterName);
                }
            }
            return paramList;
        }

        //The sql-editor clientside is based on @ to extract parameters. Oracle uses :param instead of @param, so we try to change all parameters to the oracle-name here.
        private string ParseQuery(string query, IEnumerable<DbParameter> param)
        {
            var q = query;
            if (param != null) { 
                foreach (var dbParameter in param)
                {
                    q = q.Replace(dbParameter.ParameterName, _replacer.Replace(dbParameter.ParameterName));
                }
            }
            return q;
        }

        public virtual List<DataTable> GetMultipleResults(Connection conn, string query, IEnumerable<DbParameter> param)
        {
            var parsedQuery = ParseQuery(query, param);
            var parsedParamList = ParseParameters(param);
            var tables = new List<DataTable>();
            using (var cn = GetOpenConnection(conn))
            {
                try
                {
                    var cmd = CreateCommand(cn);
                    cmd.CommandType = parsedQuery.ToLower().StartsWith("select ") ? CommandType.Text : CommandType.StoredProcedure;
                    cmd.CommandText = parsedQuery;
                    if (parsedParamList != null)
                        cmd.Parameters.AddRange(parsedParamList.ToArray());

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

        public virtual DataTable GetResults(Connection conn, string query, IEnumerable<DbParameter> param)
        {

            var parsedQuery = ParseQuery(query, param);
            var parsedParamList = ParseParameters(param);
            using (var cn = GetOpenConnection(conn))
            {
                try
                {
                    DataTable table = new DataTable();
                    var cmd = CreateCommand(cn);
                    cmd.CommandType = parsedQuery.ToLower().StartsWith("select ") ? CommandType.Text : CommandType.StoredProcedure;
                    cmd.CommandText = parsedQuery;
                    if (parsedParamList != null)
                        cmd.Parameters.AddRange(parsedParamList.ToArray());

                    using (var da = GetDataAdapter(cmd))
                    {
                        da.Fill(table);
                    }

                    return table;
                }
                catch (Exception ex)
                {
                    string hepp = ex.ToString();
                    return null;
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        private DataTable GetDataTable(DbDataReader reader)
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

        private void AddRowWithData(DataTable data, DbDataReader reader)
        {
            var newRow = data.Rows.Add();

            for (int i = 0; i < data.Columns.Count; i++)
            {
                newRow[i] = reader[i];
            }
        }

        protected DbCommand CreateCommand(DbConnection cn)
        {
            var cmd = cn.CreateCommand();
            cmd.CommandTimeout = 60;
            return cmd;
        }

        protected abstract DbDataAdapter GetDataAdapter(DbCommand cmd);
        
        protected abstract DbConnection GetOpenConnection(Connection conn);
    }
}
