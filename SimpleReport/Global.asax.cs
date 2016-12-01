using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Mvc.FilterBindingSyntax;
using Ninject.Web.WebApi;
using NLog.Internal;
using SimpleReport.App_Start;
using SimpleReport.Helpers;
using SimpleReport.Model;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Replacers;
using SimpleReport.Model.Result;
using SimpleReport.Model.Service;
using SimpleReport.Model.Storage;
using SimpleReport.Model.Storage.SQL;
using SimpleReport.Model.Subscriptions;
using ApplicationSettings = SimpleReport.Model.ApplicationSettings;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using IApplicationSettings = SimpleReport.Model.IApplicationSettings;

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
            FindAndSetLicences();
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IStorageHelper>().To<StorageHelper>();
            kernel.Bind<IStorage>().To<SQLStorage>().InRequestScope();
            kernel.Bind<ILogger>().To<Nlogger>();
            kernel.Bind<IApplicationSettings>().To<ApplicationSettings>().InSingletonScope();
            kernel.Bind<IPdfService>().To<PdfService>().InSingletonScope();
            kernel.BindFilter<HandleMyOwnErrorAttribute>(FilterScope.Controller, 0);
            kernel.Bind<ReportResolver>().ToSelf();
            kernel.Bind<IXmlReplacer>().To<XmlReplacer>();
            kernel.Bind<IScheduleRepository>().To<ScheduleRepository>().WithConstructorArgument("connectionstring", ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            kernel.Bind<ISubscriptionRepository>().To<SubscriptionRepository>().WithConstructorArgument("connectionstring", ConfigurationManager.ConnectionStrings["db"].ConnectionString);

            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
            return kernel;
        }

        protected void FindAndSetLicences()
        {
            var licensefilePath = System.Configuration.ConfigurationManager.AppSettings["licenseFilePath"];
            if (!string.IsNullOrWhiteSpace(licensefilePath) && File.Exists(licensefilePath))
            {
                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense(licensefilePath);
            }

        }
    }

}