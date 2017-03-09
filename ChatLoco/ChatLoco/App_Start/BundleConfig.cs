using System.Web;
using System.Web.Optimization;

namespace ChatLoco
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery/jquery-ui-{version}.js"));

            bundles.Add(new StyleBundle("~/bundles/accountchecking").Include(
                            "~/Scripts/Shared/accountchecking.js"));

            bundles.Add(new StyleBundle("~/bundles/layout").Include(
                            "~/Scripts/Shared/layout.js"));

            bundles.Add(new StyleBundle("~/bundles/map").Include(
                            "~/Scripts/Shared/map.js"));

            bundles.Add(new StyleBundle("~/bundles/notifications").Include(
                            "~/Scripts/Shared/notifications.js"));

            bundles.Add(new StyleBundle("~/bundles/partialview").Include(
                            "~/Scripts/Shared/partialviewhandling.js"));

            bundles.Add(new StyleBundle("~/bundles/errorhandling").Include(
                            "~/Scripts/Shared/errorhandling.js",
                            "~/Scripts/Shared/statushandling.js"));

            bundles.Add(new StyleBundle("~/bundles/user").Include(
                            "~/Scripts/User/login.js",
                            "~/Scripts/User/create.js"));

            bundles.Add(new StyleBundle("~/bundles/chatroom").Include(
                            "~/Scripts/Chatroom/chatroom.js",
                            "~/Scripts/Chatroom/index.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap/bootstrap.js",
                      "~/Scripts/bootstrap/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").IncludeDirectory("~/Content/themes/base", "*.css", true));

            bundles.Add(new StyleBundle("~/Content/chatroom").Include(
                      "~/Content/css/chatroom/chatroom.css"));

            bundles.Add(new StyleBundle("~/Content/index").Include(
                      "~/Content/css/index/index.css"));
        }
    }
}
