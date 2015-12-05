using System.Web.Http;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using WorkerWebApi.Auth;

namespace WorkerWebApi.Config
{
    public class WebPipeline
    {
        public void Configuration(IAppBuilder application)
        {
            var config = new HttpConfiguration();
            application.Use<BasicAuthMiddleware>(System.Configuration.ConfigurationManager.AppSettings["ApiUserName"], System.Configuration.ConfigurationManager.AppSettings["ApiPassword"]);
            application.UseNinjectMiddleware(CreateKernel).UseNinjectWebApi(config);

            config.MapHttpAttributeRoutes();
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
