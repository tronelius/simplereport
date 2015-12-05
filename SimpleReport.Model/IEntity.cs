using System;

namespace SimpleReport.Model
{
    public interface IEntity
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}