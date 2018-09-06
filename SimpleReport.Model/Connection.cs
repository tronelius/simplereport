using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using SimpleReport.Model.DbExecutor;

namespace SimpleReport.Model
{

    public enum ConnectionType
    {
        SQL_Server = 1,
        Oracle = 2
    }

    public class Connection : IEntity
    {
        public Guid Id { get; set; }
        public bool Verified { get; set; }

        public ConnectionType ConnectionType { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string ConnectionString { get; set; }

        public Connection(Guid id, string name, string connectionString)
        {
            Id = id;
            Name = name;
            ConnectionString = connectionString;
        }

        public Connection()
        {
            Id = Guid.NewGuid();
        }

        public ConnectionVerificationResult VerifyConnectionstring()
        {
            return DbExecutorFactory.GetInstance(this).VerifyConnectionstring(this.ConnectionString);
        }
    }
}