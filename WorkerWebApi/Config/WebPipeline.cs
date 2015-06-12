using System.Web.Http;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;

namespace WorkerWebApi.Config
{
    public class WebPipeline
    {
        public void Configuration(IAppBuilder application)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            application.UseNinjectMiddleware(CreateKernel).UseNinjectWebApi(config);
            config.EnsureInitialized();
        }

        private static StandardKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            IoCConfig.Register(kernel);
            return kernel;
        }
    }
}
