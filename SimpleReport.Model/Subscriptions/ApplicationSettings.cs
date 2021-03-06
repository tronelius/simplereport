﻿using System.Configuration;

namespace SimpleReport.Model.Subscriptions
{
    public interface IApplicationSettings
    {
        string ErrorMailRecipient { get; set; }
        string EmailFromAddress { get; set; }
    }

    public class ApplicationSettings : IApplicationSettings
    {
        public ApplicationSettings()
        {
            ErrorMailRecipient = ConfigurationManager.AppSettings["ErrorMailRecipient"];
            EmailFromAddress = ConfigurationManager.AppSettings["EmailFromAddress"];
        }

        public string ErrorMailRecipient { get; set; }
        public string EmailFromAddress { get; set; }
    }
}
