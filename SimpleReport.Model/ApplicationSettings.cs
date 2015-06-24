using System.Configuration;

namespace SimpleReport.Model
{
    public interface IApplicationSettings
    {
        bool SubscriptionEnabled { get; set; }
    }

    public class ApplicationSettings : IApplicationSettings
    {
        public ApplicationSettings()
        {
            SubscriptionEnabled = bool.Parse(ConfigurationManager.AppSettings["SubscriptionEnabled"]);
        }

        public bool SubscriptionEnabled { get; set; }
    }
}
