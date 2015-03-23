using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SimpleReport.Model
{
    
    public class Connection
    {
        public Guid Id { get; set; }
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
            Id = new Guid();
        }
    }
}