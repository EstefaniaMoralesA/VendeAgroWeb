using System.Web;
using System.Web.Optimization;

namespace VendeAgroWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/nouislider").Include(
                        "~/Scripts/nouislider.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/carousel/js").Include(
                        "~/Scripts/owl.carousel.js"));

            bundles.Add(new ScriptBundle("~/bundles/wNumb").Include(
                        "~/Scripts/wNumb.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap.min").Include(
                        "~/Scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/lightslider/js").Include(
                        "~/Scripts/lightslider.js"));

            bundles.Add(new ScriptBundle("~/bundles/lightGallery/js").Include(
                       "~/Scripts/lightGallery.js"));

            bundles.Add(new ScriptBundle("~/bundles/intro").Include(
                       "~/Scripts/intro.js"));

            bundles.Add(new ScriptBundle("~/bundles/dataTable").Include(
                       "~/Scripts/DataTables.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/nouislider").Include(
                      "~/Content/nouislider.min.css"
               ));

            bundles.Add(new StyleBundle("~/Content/ligthslider.css").Include(
                      "~/Content/lightslider.css"
               ));

            bundles.Add(new StyleBundle("~/Content/ligthGallery.css").Include(
                      "~/Content/lightGallery.css"
               ));

            bundles.Add(new StyleBundle("~/Content/introjs.css").Include(
                      "~/Content/introjs.css"
               ));

            bundles.Add(new StyleBundle("~/Content/dataTable").Include(
                      "~/Content/DataTables.min.css"
               ));


            bundles.Add(new StyleBundle("~/owl/css").Include(
                     "~/Content/owl.carousel.css",
                     "~/Content/owl.theme.css",
                     "~/Content/owl.transition.css"
                     ));
        }
    }
}
