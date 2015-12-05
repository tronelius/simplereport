using System;
using System.Collections.Generic;
using Worker.Common.Model;

namespace Worker.Common.Repository
{
    public interface ISubscriptionRepository
    {
        Subscription Get(int id);
        int Insert(Subscription schedule);
        List<Subscription> List();
        void Update(Subscription schedule);
        void Delete(int id);
        void UpdateTemplateText(UpdateTemplateText updateTemplateText);
        void SendNow(int id);
        List<Subscription> GetSubscriptionsWithSendDateBefore(DateTime now, int maxFailed);
        void DeleteAll();
    }
}