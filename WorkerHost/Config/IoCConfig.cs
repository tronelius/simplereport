using Ninject;
using Worker.Common.Api;
using Worker.Common.Common;

namespace WorkerHost.Config
{
    public static class IoCConfig
    {
        public static void Register(StandardKernel kernel)
        {
            Worker.Common.IoCConfig.Register(kernel);
            kernel.Bind<ILogger>().To<Logger>();
            kernel.Bind<IWorkerApiClient>().To<WorkerApiClient>();
        }
    }
}
