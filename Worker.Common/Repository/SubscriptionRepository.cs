using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperExtensions;
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
    }

    public class SubscriptionRepository : BaseRepository, ISubscriptionRepository
    {
        public SubscriptionRepository(string connectionstring) : base(connectionstring){}
      
        public Subscription Get(int id)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var schedule = cn.Get<Subscription>(id);
                return schedule;
            }
        }

        public int Insert(Subscription schedule)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var id = cn.Insert(schedule);
                return id;
            }
        }

        public List<Subscription> List()
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var list = cn.GetList<Subscription>();
                return list.ToList();
            }
        }

        public void Update(Subscription schedule)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                cn.Update(schedule);
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                cn.Delete<Subscription>(new { Id = id });
            }
        }

        public void UpdateTemplateText(UpdateTemplateText updateTemplateText)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                cn.Execute("Update Subscription set Mailsubject=@mailsubject, MailText=@MailText where reportid=@reportid",new {mailsubject=updateTemplateText.Subject, mailtext =updateTemplateText.Text, reportid=updateTemplateText.ReportGuid});
            }
        }

        public void SendNow(int id)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                cn.Execute("Update Subscription set NextSend = @date where Id = @Id", new { Date = DateTime.Now, Id = id});
            }
        }

        public List<Subscription> GetSubscriptionsWithSendDateBefore(DateTime now, int maxFailed)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var list = cn.Query<Subscription>("select * from subscription where NextSend < @nextsend and (FailedAttempts is null or FailedAttempts < @maxFailed)", new { NextSend = now, MaxFailed = maxFailed });
                return list.ToList();
            }
        }
    }
}
