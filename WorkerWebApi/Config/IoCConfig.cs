using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using Ninject;
using Ninject.Web.WebApi.FilterBindingSyntax;
using Worker.Common.Common;

namespace WorkerWebApi.Config
{
    public static class IoCConfig
    {
        public static void Register(StandardKernel kernel)
        {
            Worker.Common.IoCConfig.Register(kernel);
            kernel.Bind<ILogger>().To<Logger>();
            kernel.BindHttpFilter<HandleMyOwnErrorAttribute>(FilterScope.Controller);
        }

    }

    public class HandleMyOwnErrorAttribute : ExceptionFilterAttribute 
    {
        private readonly ILogger _logger;

        public HandleMyOwnErrorAttribute(ILogger logger) : base()
        {
            _logger = logger;
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);
            var controllerName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            _logger.Error(string.Format("Unhandled Exception in MVC controller={0}, Action={1}", controllerName, actionName), actionExecutedContext.Exception);
        }
    }
}
