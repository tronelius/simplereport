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
        public DesignerController(ReportResolver reportResolver, ILogger logger) : base(reportResolver.Storage, logger){}

        public ActionResult Index()
        {
            if (User.IsInRole(_reportStorage.GetSettings().AdminAccess))
                return View(GetReportViewModel());
            return Redirect("~");
        }


    }
}
