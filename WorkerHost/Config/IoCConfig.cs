using System.Configuration;
using Ninject;
using NLog;
using SimpleReport.Model.Service;
using SimpleReport.Model.Subscriptions;

namespace WorkerHost.Config
{
    public static class IoCConfig
    {
        public static void Register(StandardKernel kernel)
        {
            //Worker.Common.IoCConfig.Register(kernel);
            kernel.Bind<ILogger>().To<Logger>();
            kernel.Bind<IScheduleRepository>().To<ScheduleRepository>().WithConstructorArgument("connectionstring", ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            kernel.Bind<ISubscriptionRepository>().To<SubscriptionRepository>().WithConstructorArgument("connectionstring", ConfigurationManager.ConnectionStrings["db"].ConnectionString);

            //kernel.Bind<IWorkerApiClient>().To<WorkerApiClient>();
            kernel.Bind<IMailSender>().To<MailSender>();
            kernel.Bind<SimpleReport.Model.Subscriptions.IApplicationSettings>().To<SimpleReport.Model.Subscriptions.ApplicationSettings>().InSingletonScope();

        }
    }
}
