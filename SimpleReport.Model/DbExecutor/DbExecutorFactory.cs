using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleReport.Model.DbExecutor
{
    public static class DbExecutorFactory
    {
        public static IDbExecutor GetInstance(Connection conn)
        {
            switch (conn.ConnectionType)
            {
                case ConnectionType.SQL_Server:
                    return new DbSqlServerExecutor();
                case ConnectionType.Oracle:
                    return new DbOracleExecutor();
                default:
                    return new DbSqlServerExecutor();
            }
        }
    }
}
