﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace SimpleReport.App_Start
{       
    public class BundleConfig
        {
            // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
            public static void RegisterBundles(BundleCollection bundles)
            {
                bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                          "~/Scripts/jquery-{version}.js",
                          "~/Scripts/jquery.validate.min.js",
                          "~/Scripts/jquery.validate.unobtrusive.min.js",
                          "~/Scripts/jquery.numeric.js",
                          "~/Scripts/bootstrap.js",
                          "~/Scripts/bootstrap-datepicker.js",
                          "~/Scripts/underscore.min.js",
                          "~/Scripts/knockout-{version}.js",
                          "~/Scripts/knockout.mapping-latest.js",
                          "~/Content/js/app.js"));

                bundles.Add(new StyleBundle("~/bundles/css").Include(
                          "~/Content/bootstrap.css",
                          "~/Content/custom.css",
                          "~/Content/css/AdminLTE.css",
                          "~/Content/css/skins/_all-skins.css"));
            }
        }
    }
