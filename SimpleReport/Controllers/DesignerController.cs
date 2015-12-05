using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using SimpleReport.Model;
using SimpleReport.Model.Logging;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers
{
    public class DesignerController : BaseController
    {
        public DesignerController(ReportResolver reportResolver, ILogger logger, IApplicationSettings applicationSettings) : base(reportResolver.Storage, logger, applicationSettings) { }

        public ActionResult Index()
        {
            if (!_adminAccess.IsAvailableForMe(User)) {
               Response.Redirect("~", true);
                return new EmptyResult();
            }
            else
                return View(GetReportViewModel());
        }


    }
}
