using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Appnetwork2022
{
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              name: "Default",
              url: "{controller}/{action}/{urlVal}/{moreVal}",
              defaults: new { urlVal = UrlParameter.Optional, moreVal=UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "redirector", url: "",
              defaults: new { controller = "Access", action = "Redirector" }
            );

            routes.MapRoute(
              name: "access", url: "access",
              defaults: new { Controller = "access", action = "index" }
            );
            routes.MapRoute(
             name: "home", url: "Home",
             defaults: new { Controller = "Home", action = "index" }
            );

            routes.MapRoute(
               name: "chat", url: "Chat",
               defaults: new { Controller = "Chat", action = "index" }
            );

            routes.MapRoute(
              name: "logout", url: "Logout",
              defaults: new { Controller = "Home", action = "Logout" }
           );
            routes.MapRoute(
                name: "dashboard", url: "Dashboard",
                defaults: new { Controller = "Dashboard", action = "index" }
            );
            //  routes.MapRoute(
            //  name: "DirAuth", url: "{controller}/{action}/{id}",
            //  defaults: new { Controller = "access", action = "DirAuth" }
            //);



        }
    }
}
