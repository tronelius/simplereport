using System;
using System.Collections.Generic;

namespace SimpleReport.Model.Subscriptions
{
    public interface ISubscriptionRepository
    {
        Subscription Get(int id);
        int Insert(Subscription sub);
        List<Subscription> List();
        void Update(Subscription sub);
        void Delete(int id);
        void UpdateTemplateText(UpdateTemplateText updateTemplateText);
        void SendNow(int id);
        List<Subscription> GetSubscriptionsWithSendDateBefore(DateTime now, int maxFailed);
        void DeleteAll();
        List<Subscription> GetSubscriptionsByReportId(Guid id);
    }
}