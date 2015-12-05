using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace SimpleReport.Model
{
    public class Access : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ADGroup { get; set; }

        public string[] SplittedAdGroups
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ADGroup))
                    return ADGroup.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                return new string[0];
            } 
        }

        public bool IsAvailableForMe(IPrincipal user)
        {
            return string.IsNullOrWhiteSpace(ADGroup) || SplittedAdGroups.Any(user.IsInRole);
        }

        public void IsAllowedForMe(IPrincipal user)
        {
            if (!string.IsNullOrWhiteSpace(ADGroup) && !SplittedAdGroups.Any(user.IsInRole))
                throw new Exception("Action not allowed");

        }

        public Access(Guid id, string name, string adGroup)
        {
            Id = id;
            Name = name;
            ADGroup = adGroup;
        }
        
        //Used to dynamically create Access-objects from settings.
        public Access(string adGroup)
        {
            ADGroup = adGroup;
        }

        public Access()
        {
            Id = Guid.NewGuid();
        }
    }
}