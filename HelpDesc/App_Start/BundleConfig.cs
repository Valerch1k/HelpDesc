using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Optimization;

namespace HelpDesc.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));
            bundles.Add(new ScriptBundle("~/bundles/ckEditor").Include(
                     "~/Scripts/ckeditor/ckeditor.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js",
                         "~/Scripts/HelpDesk.js"));

            bundles.Add(new ScriptBundle("~/bundles/klorofil").Include(
                       "~/Scripts/klorofil.min.js",
                       "~/Scripts/plugins/chartist/chartist.min.js",
                       "~/Scripts/plugins/jquery-easypiechart/jquery.easypiechart.min.js",
                        "~/Scripts/plugins/jquery-slimscroll/jquery.slimscroll.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                   "~/Content/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/themes/baseall/css").Include(
                  "~/Content/themes/base/all.css",
                 "~/Content/themes/base/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                         "~/Content/themes/base/jquery.ui.all.css",
                        "~/Content/themes/base/theme.css"));

            bundles.Add(new StyleBundle("~/Content/themes/main/css").Include(
                 "~/Content/demo.css",
                 "~/Content/main.min.css",
                "~/Content/vendor/icon-sets.css"));
        }
    }
}
