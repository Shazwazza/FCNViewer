using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace FCNViewer
{
    public static class FcnExtensions
    {
        public static void MapFcnViewerRoute(this HttpRouteCollection routes, string basePath = "fcn")
        {
            routes.MapHttpRoute(
                name: "FcnViewer",
                routeTemplate: basePath,
                defaults: new { controller = "FcnViewer", id = RouteParameter.Optional }
            );
        }
    }
}
