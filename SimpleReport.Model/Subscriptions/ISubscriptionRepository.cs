using System;
using System.Collections.Generic;

namespace SimpleReport.Model.Subscriptions
{
    public interface ISubscriptionRepository
    {
        Subscription Get(Guid id);
        Guid Insert(Subscription sub);
        List<Subscription> List();
        void Update(Subscription sub);
        void Delete(Guid id);
        void SendNow(Guid id);
        List<Subscription> GetSubscriptionsWithSendDateBefore(DateTime now, int maxFailed);
        void DeleteAll();
        List<Subscription> GetSubscriptionsByReportId(Guid id);
    }
}