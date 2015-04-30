using System;
using System.Security.Principal;

namespace SimpleReport.Model
{
    public class Settings 
    {
        public string AdminAccess { get; set; }
        
        public Access AdminAccessChecker()
        {
            return new Access(AdminAccess);
        }

        //default-values 
        public Settings()
        {
            
        }
    }
}