using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker.Common.Repository
{
    public class BaseRepository 
    {
       protected readonly string _connectionString;
       protected BaseRepository(string connectionstring)
        {
            _connectionString = connectionstring;
        }

        protected SqlConnection EnsureOpenConnection()
        {
            var ensureOpenConnection = new SqlConnection(_connectionString);
            ensureOpenConnection.Open();
            return ensureOpenConnection;
        }
    }
}
