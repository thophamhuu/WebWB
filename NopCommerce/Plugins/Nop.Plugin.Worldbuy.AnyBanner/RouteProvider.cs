using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Worldbuy.AnyBanner
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var routeList = routes.MapRoute(
             "Nop.Plugin.Worldbuy.AnyBanner.List",
             "admin/AnyBanner/List",
             new { controller = "AnyBanner", action = "List" },
             new[] { "Nop.Plugin.Worldbuy.AnyBanner.Controllers" });
            routes.Remove(routeList);
            routes.Insert(0, routeList);

            var routeRead = routes.MapRoute(
             "Nop.Plugin.Worldbuy.AnyBanner.ReadBanner",
             "admin/AnyBanner/ReadBanner",
             new { controller = "AnyBanner", action = "ReadBanner" },
             new[] { "Nop.Plugin.Worldbuy.AnyBanner.Controllers" });
            routes.Remove(routeRead);
            routes.Insert(0, routeRead);

            var routeCreate = routes.MapRoute(
             "Nop.Plugin.Worldbuy.AnyBanner.Create",
             "admin/AnyBanner/Create",
             new { controller = "AnyBanner", action = "Create" },
             new[] { "Nop.Plugin.Worldbuy.AnyBanner.Controllers" });
            routes.Remove(routeCreate);
            routes.Insert(0, routeCreate);

            var routeEdit = routes.MapRoute(
             "Nop.Plugin.Worldbuy.AnyBanner.Edit",
             "admin/AnyBanner/Edit/{id}",
             new { controller = "AnyBanner", action = "Edit", id = UrlParameter.Optional },
             new[] { "Nop.Plugin.Worldbuy.AnyBanner.Controllers" });
            routes.Remove(routeEdit);
            routes.Insert(0, routeEdit);

            var routeDelete = routes.MapRoute(
             "Nop.Plugin.Worldbuy.AnyBanner.Delete",
             "admin/AnyBanner/Delete",
             new { controller = "AnyBanner", action = "Delete", id = UrlParameter.Optional },
             new[] { "Nop.Plugin.Worldbuy.AnyBanner.Controllers" });
            routes.Remove(routeDelete);
            routes.Insert(0, routeDelete);

            var routeItems = routes.MapRoute(
             "Nop.Plugin.Worldbuy.AnyBanner.Items",
             "admin/AnyBanner/Items",
             new { controller = "AnyBanner", action = "Items" },
             new[] { "Nop.Plugin.Worldbuy.AnyBanner.Controllers" });
            routes.Remove(routeItems);
            routes.Insert(0, routeItems);

            var routeCreateItem = routes.MapRoute(
             "Nop.Plugin.Worldbuy.AnyBanner.Item",
             "admin/AnyBanner/Item",
             new { controller = "AnyBanner", action = "Item" },
             new[] { "Nop.Plugin.Worldbuy.AnyBanner.Controllers" });
            routes.Remove(routeCreateItem);
            routes.Insert(0, routeCreateItem);

            var routeSaveItem = routes.MapRoute(
             "Nop.Plugin.Worldbuy.AnyBanner.SaveItem",
             "admin/AnyBanner/SaveItem",
             new { controller = "AnyBanner", action = "SaveItem" },
             new[] { "Nop.Plugin.Worldbuy.AnyBanner.Controllers" });
            routes.Remove(routeSaveItem);
            routes.Insert(0, routeSaveItem);

            var routeDeleteItem = routes.MapRoute(
             "Nop.Plugin.Worldbuy.AnyBanner.DeleteItem",
             "admin/AnyBanner/DeleteItem",
             new { controller = "AnyBanner", action = "DeleteItem", id = UrlParameter.Optional },
             new[] { "Nop.Plugin.Worldbuy.AnyBanner.Controllers" });
            routes.Remove(routeDeleteItem);
            routes.Insert(0, routeDeleteItem);
        }
        public int Priority
        {
            get { return 0; }
        }
    }
}
