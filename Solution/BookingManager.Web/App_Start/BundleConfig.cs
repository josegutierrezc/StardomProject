using System.Web;
using System.Web.Optimization;

namespace BookingManager.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            // Inspinia script
            bundles.Add(new ScriptBundle("~/bundles/inspinia").Include(
                      "~/Scripts/app/inspinia.js"));

            // BookingManager script
            bundles.Add(new ScriptBundle("~/bundles/bookingmanager").Include(
                      "~/Scripts/app/bookingmanager.js"));

            // SlimScroll
            bundles.Add(new ScriptBundle("~/plugins/slimScroll").Include(
                      "~/Scripts/plugins/slimScroll/jquery.slimscroll.min.js"));

            // jQuery plugins
            bundles.Add(new ScriptBundle("~/plugins/metsiMenu").Include(
                      "~/Scripts/plugins/metisMenu/metisMenu.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/pace").Include(
                      "~/Scripts/plugins/pace/pace.min.js"));

            bundles.Add(new ScriptBundle("~/plugins/datePicker").Include(
                      "~/Scripts/plugins/datepicker/bootstrap-datepicker.js"));

            bundles.Add(new ScriptBundle("~/plugins/jasny").Include(
                      "~/Scripts/plugins/jasny/jasny-bootstrap.min.js"));

            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/plugins/footable/footable.bootstrap.css",
                      "~/Content/plugins/datepicker/bootstrap-datepicker3.standalone.css",
                      "~/Content/plugins/touchspin/jquery.bootstrap-touchspin.css",
                      "~/Content/plugins/toastr/toastr.min.css",
                      "~/Content/plugins/awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css",
                      "~/Content/plugins//jasny/jasny-bootstrap.min.css",
                      "~/Content/style.css"));

            // Footable alert
            bundles.Add(new ScriptBundle("~/plugins/footable").Include(
                      "~/Scripts/moments.js",
                      "~/Scripts/plugins/footable/footable.js"));

            // Touch Spin
            bundles.Add(new ScriptBundle("~/plugins/touchspin").Include(
                      "~/Scripts/moments.js",
                      "~/Scripts/plugins/touchspin/jquery.bootstrap-touchspin.js"));

            // Notifications
            bundles.Add(new ScriptBundle("~/plugins/toastr").Include(
                      "~/Scripts/plugins/toastr/toastr.min.js"));

            // Charts
            bundles.Add(new ScriptBundle("~/plugins/charts").Include(
                      "~/Scripts/plugins/chartjs/Chart.min.js"));

            // Font Awesome icons
            bundles.Add(new StyleBundle("~/font-awesome/css").Include(
                      "~/fonts/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));

            // Pages Scripts
            bundles.Add(new ScriptBundle("~/app/carreservations").Include(
                      "~/Scripts/app/carreservations.js"));
            bundles.Add(new ScriptBundle("~/app/editcarreservation").Include(
                      "~/Scripts/app/editcarreservation.js"));
            bundles.Add(new ScriptBundle("~/app/viewclient").Include(
                      "~/Scripts/app/viewclient.js"));
            bundles.Add(new ScriptBundle("~/app/clients").Include(
                      "~/Scripts/app/clients.js"));
            bundles.Add(new ScriptBundle("~/app/addcarreservation").Include(
                      "~/Scripts/app/addcarreservation.js"));
            bundles.Add(new ScriptBundle("~/app/addclient").Include(
                      "~/Scripts/app/addclient.js"));
            bundles.Add(new ScriptBundle("~/app/editclient").Include(
                      "~/Scripts/app/editclient.js"));
            bundles.Add(new ScriptBundle("~/app/priceconfiguration").Include(
                      "~/Scripts/app/priceconfiguration.js"));
            bundles.Add(new ScriptBundle("~/app/carreports").Include(
                      "~/Scripts/app/carreports.js"));
            bundles.Add(new ScriptBundle("~/app/index").Include(
                      "~/Scripts/app/index.js"));
            bundles.Add(new ScriptBundle("~/app/linkedagencies").Include(
                      "~/Scripts/app/linkedagencies.js"));
            bundles.Add(new ScriptBundle("~/app/agents").Include(
                      "~/Scripts/app/agents.js"));

            //DEFAULT STUFF CREATED
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));
        }
    }
}
