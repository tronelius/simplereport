using System.Web.Mvc;
using SimpleReport.Model;
using SimpleReport.Model.Logging;

namespace SimpleReport.Controllers
{
    public class SubscriptionController : BaseController
    {
        public SubscriptionController(ReportResolver reportResolver, ILogger logger, IApplicationSettings applicationSettings) : base(reportResolver.Storage, logger, applicationSettings) { }

        public ActionResult Index()
        {
            if (_subscriptionAccess.IsAllowedToSeeSubscriptions(User)) return View(GetReportViewModel());
            if (_adminAccess.IsAvailableForMe(User)) return View(GetReportViewModel());
            Response.Redirect("~", true);
            return new EmptyResult();
        }
    }
}
