using System.Configuration;
using Ninject;
using Worker.Common.Repository;
using Worker.Common.Service;

namespace Worker.Common
{
    public static class IoCConfig
    {
        public static void Register(StandardKernel kernel)
        {
            //Maybe like this?
            kernel.Bind<IScheduleRepository>().To<ScheduleRepository>().WithConstructorArgument("connectionstring", ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            kernel.Bind<ISubscriptionRepository>().To<SubscriptionRepository>().WithConstructorArgument("connectionstring", ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            kernel.Bind<IMailSender>().To<MailSender>();
        }
    }
}
