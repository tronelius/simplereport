using System.Runtime.Serialization;

namespace SimpleReport.Model
{
    
    public class Connection
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }

        public Connection(string name, string connectionString)
        {
            Name = name;
            ConnectionString = connectionString;
        }

        public Connection()
        {
        }

    }
}