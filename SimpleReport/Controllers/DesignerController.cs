using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        public ActionResult Save(Report reportToSave)
        {
            _reportManager.SaveReport(reportToSave);
            return Index();
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
