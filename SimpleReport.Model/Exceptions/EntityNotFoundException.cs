using System;

namespace SimpleReport.Model.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string message) : base(message) {}
    }
}