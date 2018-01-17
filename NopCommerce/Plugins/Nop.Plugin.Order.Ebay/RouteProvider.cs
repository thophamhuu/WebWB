using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Order.Ebay
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var routeListOrder = routes.MapRoute("Plugin.Order.Ebay.ListOrder",
                 "Admin/OrderEbay/ListOrder",
                 new { controller = "OrderEbay", action = "ListOrder" },
                 new[] { "Nop.Plugin.Order.Ebay.Controllers" }
            );
            routes.Remove(routeListOrder);
            routes.Insert(0, routeListOrder);

           // var routeGetProductTest = routes.MapRoute("Plugin.Order.Ebay.GetProductTest",
           //     "Admin/OrderEbay/GetProductTest",
           //     new { controller = "OrderEbay", action = "GetProductTest" },
           //     new[] { "Nop.Plugin.Order.Ebay.Controllers" }
           //);
           // routes.Remove(routeGetProductTest);
           // routes.Insert(0, routeGetProductTest);

            var routeListOrderEbay = routes.MapRoute("Plugin.Order.Ebay.ListOrderEbay",
                  "Admin/OrderEbay/ListOrderEbay",
                  new { controller = "OrderEbay", action = "ListOrderEbay" },
                  new[] { "Nop.Plugin.Order.Ebay.Controllers" }
             );
            routes.Remove(routeListOrderEbay);
            routes.Insert(0, routeListOrderEbay);

            var routeDetailOrderEbay = routes.MapRoute("Plugin.Order.Ebay.Edit",
                 "Admin/OrderEbay/Edit",
                 new { controller = "OrderEbay", action = "Edit" },
                 new[] { "Nop.Plugin.Order.Ebay.Controllers" }
            );
            routes.Remove(routeDetailOrderEbay);
            routes.Insert(0, routeDetailOrderEbay);
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
