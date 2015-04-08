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

        public BaseController(IStorage reportStorage, ILogger logger)
        {
            _reportStorage = reportStorage;
            _logger = logger;
        }

        protected void HasAdminAccess()
        {
            var adminaccess = _reportStorage.GetSettings().AdminAccess;
            if (!User.IsInRole(adminaccess) && !string.IsNullOrWhiteSpace(adminaccess))
            Redirect("~");
        }


        protected ReportViewModel GetReportViewModel()
        {
            ReportViewModel vm = new ReportViewModel();
            vm.AdminRole = _reportStorage.GetSettings().AdminAccess;
            vm.Reports = _reportStorage.GetAllReports().Where(a => a.IsAvailableForMe(User, _reportStorage.GetSettings().AdminAccess));
            return vm;
        }
        
    }
}