using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SimpleReport.Model;
using SimpleReport.Model.Storage;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IStorage _reportStorage;

        public BaseController(IStorage reportStorage)
        {
            _reportStorage = reportStorage;
        }

        protected ReportViewModel GetReportViewModel()
        {
            ReportViewModel vm = new ReportViewModel();
            vm.Reports = _reportStorage.GetAllReports().Where(a => a.IsAvailableForMe(User, _reportStorage.GetSettings().AdminAccess));
            return vm;
        }
        
    }
}