using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace WorkerHost
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<SubscriptionWorker>(s =>
                {
                    s.ConstructUsing(name => new SubscriptionWorker());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.UseNLog();

                x.SetDescription("SimpleReport.Subscriptions");
                x.SetDisplayName(ConfigurationManager.AppSettings["ServiceName"]);
                x.SetServiceName(ConfigurationManager.AppSettings["ServiceName"]);
            });
        }
    }
}
