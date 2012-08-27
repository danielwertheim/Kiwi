using System.Web.Mvc;
using System.Web.Routing;

[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.KiwiMvc3PackageInit), "PreStart")]

namespace $rootnamespace$.App_Start
{
    public static class KiwiMvc3PackageInit
    {
        public static void PreStart()
        {
            RegisterRoutes(RouteTable.Routes);
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Wiki",
                "wiki/{*docId}",
                new { controller = "wiki", action = "doc", docId = "home" });
        }
    }
}