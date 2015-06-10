using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SimpleReport.Model;
using SimpleReport.Model.Logging;
using SimpleReport.Model.Storage;
using SimpleReport.ViewModel;

namespace SimpleReport.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IStorage _reportStorage;
        protected readonly ILogger _logger;
        protected readonly Access _adminAccess;

        public BaseController(IStorage reportStorage, ILogger logger)
        {
            _reportStorage = reportStorage;
            _logger = logger;
            _adminAccess = _reportStorage.GetSettings().AdminAccessChecker();

        }

        protected ReportViewModel GetReportViewModel()
        {
            ReportViewModel vm = new ReportViewModel();
            vm.HasAdminAccess = _reportStorage.GetSettings().AdminAccessChecker().IsAvailableForMe(User);
            vm.Reports = _reportStorage.GetAllReports().Where(a => a.IsAvailableForMe(User, _adminAccess)).OrderBy(a => a.Group).ThenBy(b => b.Name);
            return vm;
        }
        
    }
}