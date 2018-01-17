using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Affiliate.CategoryMap
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var routeCategory = routes.MapRoute(
             "Nop.Plugin.Affiliate.ProductMapping.LoadUrl",
             "admin/ProductMapping/LoadUrl",
             new { controller = "ProductMapping", action = "LoadUrl" },
             new[] { "Nop.Plugin.Affiliate.CategoryMap.Controllers" });
            routes.Remove(routeCategory);
            routes.Insert(0, routeCategory);

            var routeShipping = routes.MapRoute(
          "Nop.Plugin.Affiliate.ProductMapping.LoadShippingDescription",
          "ProductMapping/LoadShippingDescription",
          new { controller = "ProductMapping", action = "LoadShippingDescription" },
          new[] { "Nop.Plugin.Affiliate.CategoryMap.Controllers" });
            routes.Remove(routeShipping);
            routes.Insert(0, routeShipping);

            var routeUpdatePrice = routes.MapRoute(
            "Nop.Plugin.Affiliate.ProductMapping.UpdatePrice",
            "admin/ProductMapping/UpdatePrice",
            new { controller = "ProductMapping", action = "UpdatePrice" },
            new[] { "Nop.Plugin.Affiliate.CategoryMap.Controllers" });
            routes.Remove(routeUpdatePrice);
            routes.Insert(0, routeUpdatePrice);
        }
        public int Priority
        {
            get { return 0; }
        }
    }
}
