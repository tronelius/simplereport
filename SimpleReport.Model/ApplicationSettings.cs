using System.Configuration;

namespace SimpleReport.Model
{
    public interface IApplicationSettings
    {
        bool SubscriptionEnabled { get; set; }
        int SqlTimeout { get; set; }
    }

    public class ApplicationSettings : IApplicationSettings
    {
        public ApplicationSettings()
        {
            SubscriptionEnabled = bool.Parse(ConfigurationManager.AppSettings["SubscriptionEnabled"]);
            var sqlTimeoutString = ConfigurationManager.AppSettings["SqlTimeout"];
            SqlTimeout = !string.IsNullOrEmpty(sqlTimeoutString) ? int.Parse(sqlTimeoutString): 60;
        }

        public bool SubscriptionEnabled { get; set; }
        public int SqlTimeout { get; set; }
    }
}
