using System.Configuration;
using Topshelf;
using SimpleReport.Model.Migrations;

namespace WorkerHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var migration = new Migrator(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            migration.Up();

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
