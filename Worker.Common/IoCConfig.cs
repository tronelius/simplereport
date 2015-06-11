using System.Configuration;
using Ninject;
using Worker.Common.Repository;

namespace Worker.Common
{
    public static class IoCConfig
    {
        public static void Register(StandardKernel kernel)
        {
            //Maybe like this?
            kernel.Bind<IScheduleRepository>().To<ScheduleRepository>().WithConstructorArgument("connectionstring", ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        }
    }
}
