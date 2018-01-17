using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Worldbuy.StateProvinceWB
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var routeList = routes.MapRoute(
              "Nop.Plugin.Worldbuy.StateProvinceWB.List",
              "admin/StateProvinceWB/List",
              new { controller = "StateProvinceWB", action = "List" },
              new[] { "Nop.Plugin.Worldbuy.StateProvinceWB.Controllers" });
            routes.Remove(routeList);
            routes.Insert(0, routeList);

            var routeRead = routes.MapRoute(
              "Nop.Plugin.Worldbuy.StateProvinceWB.Read",
              "admin/StateProvinceWB/Read",
              new { controller = "StateProvinceWB", action = "Read" },
              new[] { "Nop.Plugin.Worldbuy.StateProvinceWB.Controllers" });
            routes.Remove(routeRead);
            routes.Insert(0, routeRead);

            var routeUpdate = routes.MapRoute(
              "Nop.Plugin.Worldbuy.StateProvinceWB.Update",
              "admin/StateProvinceWB/Update",
              new { controller = "StateProvinceWB", action = "Update" },
              new[] { "Nop.Plugin.Worldbuy.StateProvinceWB.Controllers" });
            routes.Remove(routeUpdate);
            routes.Insert(0, routeUpdate);

            var routeDelete = routes.MapRoute(
              "Nop.Plugin.Worldbuy.StateProvinceWB.Delete",
              "admin/StateProvinceWB/Delete",
              new { controller = "StateProvinceWB", action = "Delete" },
              new[] { "Nop.Plugin.Worldbuy.StateProvinceWB.Controllers" });
            routes.Remove(routeDelete);
            routes.Insert(0, routeDelete);

            var routeSync = routes.MapRoute(
             "Nop.Plugin.Worldbuy.StateProvinceWB.ImportFromXlsx",
             "admin/StateProvinceWB/ImportFromXlsx",
             new { controller = "StateProvinceWB", action = "ImportFromXlsx" },
             new[] { "Nop.Plugin.Worldbuy.StateProvinceWB.Controllers" });
            routes.Remove(routeSync);
            routes.Insert(0, routeSync);
        }
        public int Priority
        {
            get { return 0; }
        }
    }
}
