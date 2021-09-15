using System.Collections.Generic;
using System.Security.Principal;

namespace SimpleReport.Model
{
    public class Settings 
    {
        public string AdminAccess { get; set; }
        public string SubscriptionAccess { get; set; }
        
        public Access AdminAccessChecker()
        {
            return new Access(AdminAccess);
        }

        public Access SubscriptionSettingAccessChecker()
        {
            return new Access(SubscriptionAccess);
        }

        public void AddCurrentUserToAdminAccess(IPrincipal user)
        {
            var currentLoggedInUser = user.Identity.Name;
            if (!this.AdminAccess.Contains(currentLoggedInUser))
                this.AdminAccess += ";" + currentLoggedInUser;
        }

        //default-values 
        public Settings()
        {
            
        }

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string,string> dict = new Dictionary<string, string>();
            dict.Add("AdminAccess",AdminAccess);
            dict.Add("SubscriptionAccess", SubscriptionAccess);
            return dict;
        }

        public static Settings CreateSettingsFromDictionary(Dictionary<string, string> fromDictionary)
        {
            Settings settings = new Settings();
            if (fromDictionary.ContainsKey("AdminAccess"))
                settings.AdminAccess = fromDictionary["AdminAccess"];
            if (fromDictionary.ContainsKey("SubscriptionAccess"))
                settings.SubscriptionAccess = fromDictionary["SubscriptionAccess"];
            return settings;
        }

    }
}