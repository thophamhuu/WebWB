using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Worldbuy.SimpleMenu
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var routeList = routes.MapRoute(
             "Nop.Plugin.Worldbuy.SimpleMenu.List",
             "admin/SimpleMenu/List",
             new { controller = "SimpleMenu", action = "List" },
             new[] { "Nop.Plugin.Worldbuy.SimpleMenu.Controllers" });
            routes.Remove(routeList);
            routes.Insert(0, routeList);

            var routeRead = routes.MapRoute(
             "Nop.Plugin.Worldbuy.SimpleMenu.ReadMenu",
             "admin/SimpleMenu/ReadMenu",
             new { controller = "SimpleMenu", action = "ReadMenu" },
             new[] { "Nop.Plugin.Worldbuy.SimpleMenu.Controllers" });
            routes.Remove(routeRead);
            routes.Insert(0, routeRead);

            var routeCreate = routes.MapRoute(
             "Nop.Plugin.Worldbuy.SimpleMenu.Create",
             "admin/SimpleMenu/Create",
             new { controller = "SimpleMenu", action = "Create" },
             new[] { "Nop.Plugin.Worldbuy.SimpleMenu.Controllers" });
            routes.Remove(routeCreate);
            routes.Insert(0, routeCreate);

            var routeEdit = routes.MapRoute(
             "Nop.Plugin.Worldbuy.SimpleMenu.Edit",
             "admin/SimpleMenu/Edit/{id}",
             new { controller = "SimpleMenu", action = "Edit", id = UrlParameter.Optional },
             new[] { "Nop.Plugin.Worldbuy.SimpleMenu.Controllers" });
            routes.Remove(routeEdit);
            routes.Insert(0, routeEdit);

            var routeDelete = routes.MapRoute(
             "Nop.Plugin.Worldbuy.SimpleMenu.Delete",
             "admin/SimpleMenu/Delete",
             new { controller = "SimpleMenu", action = "Delete", id = UrlParameter.Optional },
             new[] { "Nop.Plugin.Worldbuy.SimpleMenu.Controllers" });
            routes.Remove(routeDelete);
            routes.Insert(0, routeDelete);

            var routeItems = routes.MapRoute(
             "Nop.Plugin.Worldbuy.SimpleMenu.Items",
             "admin/SimpleMenu/Items",
             new { controller = "SimpleMenu", action = "Items" },
             new[] { "Nop.Plugin.Worldbuy.SimpleMenu.Controllers" });
            routes.Remove(routeItems);
            routes.Insert(0, routeItems);

            var routeCreateItem = routes.MapRoute(
             "Nop.Plugin.Worldbuy.SimpleMenu.Item",
             "admin/SimpleMenu/Item",
             new { controller = "SimpleMenu", action = "Item" },
             new[] { "Nop.Plugin.Worldbuy.SimpleMenu.Controllers" });
            routes.Remove(routeCreateItem);
            routes.Insert(0, routeCreateItem);

            var routeSaveItem = routes.MapRoute(
             "Nop.Plugin.Worldbuy.SimpleMenu.SaveItem",
             "admin/SimpleMenu/SaveItem",
             new { controller = "SimpleMenu", action = "SaveItem" },
             new[] { "Nop.Plugin.Worldbuy.SimpleMenu.Controllers" });
            routes.Remove(routeSaveItem);
            routes.Insert(0, routeSaveItem);

            var routeDeleteItem = routes.MapRoute(
             "Nop.Plugin.Worldbuy.SimpleMenu.DeleteItem",
             "admin/SimpleMenu/DeleteItem",
             new { controller = "SimpleMenu", action = "DeleteItem", id = UrlParameter.Optional },
             new[] { "Nop.Plugin.Worldbuy.SimpleMenu.Controllers" });
            routes.Remove(routeDeleteItem);
            routes.Insert(0, routeDeleteItem);
        }
        public int Priority
        {
            get { return 0; }
        }
    }
}
