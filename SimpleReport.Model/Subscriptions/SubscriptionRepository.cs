using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using DapperExtensions;
using SimpleReport.Model.Storage.SQL;

namespace SimpleReport.Model.Subscriptions
{
    public class SubscriptionRepository : BaseDapperRepo, ISubscriptionRepository
    {
        public SubscriptionRepository(string connectionstring) : base(connectionstring) { }

        public Subscription Get(int id)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var schedule = cn.Get<Subscription>(id);
                return schedule;
            }
        }

        public int Insert(Subscription sub)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var id = cn.Insert(sub);
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

        public void Update(Subscription sub)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                cn.Update(sub);
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
                cn.Execute("Update Subscription set Mailsubject=@mailsubject, MailText=@MailText where reportid=@reportid", new { mailsubject = updateTemplateText.Subject, mailtext = updateTemplateText.Text, reportid = updateTemplateText.ReportGuid });
            }
        }

        public void SendNow(int id)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                cn.Execute("Update Subscription set NextSend = @date where Id = @Id", new { Date = DateTime.Now, Id = id });
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

        public void DeleteAll()
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                cn.Execute("Delete from Subscription");
            }
        }

        public List<Subscription> GetSubscriptionsByReportId(Guid id)
        {
            using (SqlConnection cn = EnsureOpenConnection())
            {
                var list = cn.Query<Subscription>("select * from subscription where reportId=@reportId", new { reportId = id });
                return list.ToList();
            }
        }
    }
}
