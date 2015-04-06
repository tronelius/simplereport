using System;

namespace SimpleReport.Model.Storage
{
    public class StorageNotInitializedException : Exception
    {

        public StorageNotInitializedException(Exception ex) : base("Storage not initialized",ex)
        {
            
        }
    }
}