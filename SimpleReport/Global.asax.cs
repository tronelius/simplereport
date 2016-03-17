using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Mvc.FilterBindingSyntax;
using Ninject.Web.WebApi;
using SimpleReport.App_Start;
using SimpleReport.Helpers;
using SimpleReport.Model;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Result;
using SimpleReport.Model.Storage;
using SimpleReport.Model.Storage.SQL;

namespace SimpleReport
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IStorageHelper>().To<StorageHelper>();
            kernel.Bind<IStorage>().To<SQLStorage>().InRequestScope();
            kernel.Bind<ILogger>().To<Nlogger>();
            kernel.Bind<IApiClient>().To<ApiClient>();
            kernel.Bind<IApplicationSettings>().To<ApplicationSettings>().InSingletonScope();
            kernel.BindFilter<HandleMyOwnErrorAttribute>(FilterScope.Controller, 0);
            kernel.Bind<ReportResolver>().ToSelf();
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
            return kernel;
        }

    }
}