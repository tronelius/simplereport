using System;
using System.Web.Optimization;

namespace SimpleReport.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var appbundle = new ScriptBundle("~/bundles/app").IncludeDirectory("~/Scripts/app", "*.js", true);
            appbundle.Orderer = new ModuleFirstBundleOrderer();
            bundles.Add(appbundle);

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                "~/Scripts/lib/jquery-{version}.js",
                "~/Scripts/lib/jquery.numeric.js",
                "~/Scripts/lib/bootstrap.js",
                "~/Scripts/lib/bootstrap-datepicker.js",
                "~/Scripts/lib/underscore.min.js",
                "~/Scripts/lib/angular.js",
                "~/Scripts/lib/angular-sanitize.min.js",
                "~/Scripts/lib/angular-ui/ui-bootstrap-tpls.min.js",
                "~/Scripts/lib/toastr.js",
                "~/Scripts/lib/ng-file-upload-all.min.js",
                "~/Scripts/lib/jquery-cron-min.js",
                "~/Scripts/lib/bootstrap-multiselect.js",
                "~/Content/js/app.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/custom.css",
                      "~/Content/css/AdminLTE.min.css",
                      "~/Content/css/skins/_all-skins.css",
                      "~/Content/toastr.min.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/jquery-cron.css",
                      "~/Content/bootstrap-multiselect.css",
                      "~/Content/ui-bootstrap-csp.css"));
        }
    }
}
