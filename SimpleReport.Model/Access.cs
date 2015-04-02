using System;

namespace SimpleReport.Model
{
    public class Access : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ADGroup { get; set; }

        public Access(Guid id, string name, string adGroup)
        {
            Id = id;
            Name = name;
            ADGroup = adGroup;
        }

        public Access()
        {
            Id = Guid.NewGuid();
        }
    }
}