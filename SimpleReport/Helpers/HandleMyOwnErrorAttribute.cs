using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleReport.Model.Logging;

namespace SimpleReport.Helpers
{
    public class HandleMyOwnErrorAttribute : HandleErrorAttribute
    {
        private readonly ILogger _logger;

        public HandleMyOwnErrorAttribute(ILogger logger) : base()
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            
            base.OnException(filterContext);
            var controllerName = filterContext.RouteData.Values["controller"];
            var actionName = filterContext.RouteData.Values["action"];
            _logger.Error(string.Format("Unhandled Exception in MVC controller={0}, Action={1}", controllerName, actionName), filterContext.Exception);
        }
    }
}