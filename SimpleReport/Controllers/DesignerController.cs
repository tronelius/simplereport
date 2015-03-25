﻿using System.Collections.Generic;
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
            return View(GetReportViewModel());
        }


    }
}
