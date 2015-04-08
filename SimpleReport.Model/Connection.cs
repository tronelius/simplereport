using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace SimpleReport.Model
{

    public class Connection : IEntity
    {
        public Guid Id { get; set; }
        public bool Verified { get; set; }

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
            try
            {
                string tempConnString = ConnectionString;
                if (tempConnString.ToLower().IndexOf("connection timeout=") < 0)
                    tempConnString += ";connection timeout=2";
                using (SqlConnection conn = new SqlConnection(tempConnString))
                {
                    
                    conn.Open(); // throws if invalid
                }
                return new ConnectionVerificationResult(true, "OK");
            }
            catch (Exception ex)
            {
                return new ConnectionVerificationResult(false, ex.Message);
            }
        }
    }

    public class ConnectionVerificationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ConnectionVerificationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}