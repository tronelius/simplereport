using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SimpleReport.Model.DbExecutor
{
    public interface IDbExecutor
    {
        List<DataTable> GetMultipleResults(Connection conn, string query, IEnumerable<DbParameter> param);
        DataTable GetResults(Connection conn, string query, IEnumerable<DbParameter> param);

        ConnectionVerificationResult VerifyConnectionstring(string connectionString);
        DbParameter CreateStringParameter(string name, int length);
        DbParameter CreateParameter(string key, object value);
    }
}