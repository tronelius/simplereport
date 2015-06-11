using Ninject;

namespace WorkerHost.Config
{
    class IoCConfig
    {
        public static void Register(StandardKernel kernel)
        {
            kernel.Bind<ILogger>().To<Logger>();
        }
    }
}
