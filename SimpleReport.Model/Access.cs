using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Xml.Schema;

namespace SimpleReport.Model
{
    public class Access : IEntity
    {
        public Guid Id { get; set; }

        [Required][StringLength(1000)]
        public string Name { get; set; }
        [Required][StringLength(3000)]
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
            return ConfigurationManager.AppSettings["DeveloperMode"] == "true" || string.IsNullOrWhiteSpace(ADGroup) || SplittedAdGroups.Any(user.IsInRole);
        }

        public void IsAllowedForMe(IPrincipal user)
        {
            if (!string.IsNullOrWhiteSpace(ADGroup) && !SplittedAdGroups.Any(user.IsInRole))
                throw new Exception("Action not allowed");

        }

        public bool IsAllowedToSeeSubscriptions(IPrincipal user)
        {
            if ((string.IsNullOrWhiteSpace(ADGroup)) || (!string.IsNullOrWhiteSpace(ADGroup) && !SplittedAdGroups.Any(user.IsInRole)))
                return false;
            return true;
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