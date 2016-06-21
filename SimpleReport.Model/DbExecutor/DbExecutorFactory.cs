using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleReport.Model.Replacers;

namespace SimpleReport.Model.DbExecutor
{
    public static class DbExecutorFactory
    {
        public static IDbExecutor GetInstance(Connection conn)
        {
            switch (conn.ConnectionType)
            {
                case ConnectionType.SQL_Server:
                    return new DbSqlServerExecutor(new NoReplacer());
                case ConnectionType.Oracle:
                    return new DbOracleExecutor(new OracleReservedWordsReplacer());
                default:
                    return new DbSqlServerExecutor(new NoReplacer());
            }
        }
    }
}
