using System.Configuration;
using Ninject;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Service;
using SimpleReport.Model.Subscriptions;

namespace SimpleReport.SubscriptionService.Config
{
    public static class IoCConfig
    {
        public static void Register(StandardKernel kernel)
        {
            kernel.Bind<ILogger>().To<Nlogger>();
            kernel.Bind<IScheduleRepository>().To<ScheduleRepository>().WithConstructorArgument("connectionstring", ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            kernel.Bind<ISubscriptionRepository>().To<SubscriptionRepository>().WithConstructorArgument("connectionstring", ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            kernel.Bind<IWorkerApiClient>().To<WorkerApiClient>();
            kernel.Bind<IMailSender>().To<MailSender>();
            kernel.Bind<IApplicationSettings>().To<ApplicationSettings>().InSingletonScope();

        }
    }
}
