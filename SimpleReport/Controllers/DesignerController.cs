using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using SimpleReport.Model;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers
{
    public class DesignerController : BaseController
    {
        public DesignerController(ReportManager reportManager) : base(reportManager){}

        public ActionResult Index()
        {
            var vm = GetReportViewModel();
            return View(vm);
        }

        private DesignerViewModel GetReportViewModel()
        {
            DesignerViewModel vm = new DesignerViewModel();
            vm.Reports = _reportManager.GetReports().ToList();
            if (vm.Reports.Count() > 0)
                vm.Report = vm.Reports[0];
            else
                vm.Report = null;
            return vm;
        }

       


    }
}
