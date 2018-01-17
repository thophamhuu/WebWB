using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Order.Amazon
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {

            var routeList = routes.MapRoute(
              "Nop.Plugin.Order.Amazon.List",
              "admin/OrderAmazon/List",
              new { controller = "OrderAmazon", action = "List" },
              new[] { "Nop.Plugin.Order.Amazon.Controllers" });
            routes.Remove(routeList);
            routes.Insert(0, routeList);

            var routeOrderList = routes.MapRoute(
             "Nop.Plugin.Order.Amazon.OrderList",
             "admin/OrderAmazon/OrderList",
             new { controller = "OrderAmazon", action = "OrderList" },
             new[] { "Nop.Plugin.Order.Amazon.Controllers" });
            routes.Remove(routeOrderList);
            routes.Insert(0, routeOrderList);

            var routeOrder = routes.MapRoute(
            "Nop.Plugin.Order.Amazon.Order",
            "admin/OrderAmazon/Order/{id}",
            new { controller = "OrderAmazon", action = "Order",id= UrlParameter.Optional },
            new[] { "Nop.Plugin.Order.Amazon.Controllers" });
            routes.Remove(routeOrder);
            routes.Insert(0, routeOrder);

            var routeCreate = routes.MapRoute(
          "Nop.Plugin.Order.Amazon.CartCreate",
          "admin/OrderAmazon/CartCreate",
          new { controller = "OrderAmazon", action = "CartCreate" },
          new[] { "Nop.Plugin.Order.Amazon.Controllers" });
            routes.Remove(routeCreate);
            routes.Insert(0, routeCreate);
        }
        public int Priority
        {
            get { return 0; }
        }
    }
}
