using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Affiliate.Amazon
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var routeTask = routes.MapRoute(
            "Nop.Plugin.Affiliate.Amazon.Task",
            "admin/AffiliateAmazon/Task",
            new { controller = "AffiliateAmazon", action = "Task" },
            new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeTask);
            routes.Insert(0, routeTask);
            var routeCategory = routes.MapRoute(
             "Nop.Plugin.Affiliate.Amazon.Category",
             "admin/AffiliateAmazon/Category",
             new { controller = "AffiliateAmazon", action = "Category" },
             new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeCategory);
            routes.Insert(0, routeCategory);

            var routeReadCategory = routes.MapRoute(
            "Nop.Plugin.Affiliate.Amazon.ReadCategory",
            "admin/AffiliateAmazon/ReadCategory",
            new { controller = "AffiliateAmazon", action = "ReadCategory" },
            new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeReadCategory);
            routes.Insert(0, routeReadCategory);

            var routeImportCategory = routes.MapRoute(
             "Nop.Plugin.Affiliate.Amazon.ImportCategory",
             "admin/AffiliateAmazon/ImportCategoryFromXlsx",
             new { controller = "AffiliateAmazon", action = "ImportCategoryFromXlsx" },
             new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeImportCategory);
            routes.Insert(0, routeImportCategory);

            var routeSyncCategory = routes.MapRoute(
            "Nop.Plugin.Affiliate.Amazon.SyncCategory",
            "admin/AffiliateAmazon/SyncCategory",
            new { controller = "AffiliateAmazon", action = "SyncCategory" },
            new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeSyncCategory);
            routes.Insert(0, routeSyncCategory);

            var routeMapCategory = routes.MapRoute(
            "Nop.Plugin.Affiliate.Amazon.MapCategory",
            "admin/AffiliateAmazon/MapCategory",
            new { controller = "AffiliateAmazon", action = "MapCategory" },
            new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeMapCategory);
            routes.Insert(0, routeMapCategory);

            var routeSyncProduct = routes.MapRoute(
              "Nop.Plugin.Affiliate.Amazon.SyncProduct",
              "admin/AffiliateAmazon/SyncProduct",
              new { controller = "AffiliateAmazon", action = "SyncProduct" },
              new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeSyncProduct);
            routes.Insert(0, routeSyncProduct);

            var routeLoadBrowseNode = routes.MapRoute(
             "Nop.Plugin.Affiliate.Amazon.LoadBrowseNode",
             "admin/AffiliateAmazon/LoadBrowseNode",
             new { controller = "AffiliateAmazon", action = "LoadBrowseNode" },
             new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeLoadBrowseNode);
            routes.Insert(0, routeLoadBrowseNode);

            var routeProduct = routes.MapRoute(
              "Nop.Plugin.Affiliate.Amazon.Product",
              "admin/AffiliateAmazon/Product",
              new { controller = "AffiliateAmazon", action = "Product" },
              new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeProduct);
            routes.Insert(0, routeProduct);

            var routeReadProduct = routes.MapRoute(
              "Nop.Plugin.Affiliate.Amazon.ReadProduct",
              "admin/AffiliateAmazon/ReadProduct",
              new { controller = "AffiliateAmazon", action = "ReadProduct" },
              new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeReadProduct);
            routes.Insert(0, routeReadProduct);

            var routeAccounts = routes.MapRoute(
             "Nop.Plugin.Affiliate.Amazon.Accounts",
             "admin/AffiliateAmazon/Accounts",
             new { controller = "AffiliateAmazon", action = "Accounts" },
             new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeAccounts);
            routes.Insert(0, routeAccounts);

            var routeCreateAccount = routes.MapRoute(
            "Nop.Plugin.Affiliate.Amazon.CreateAccount",
            "admin/AffiliateAmazon/CreateAccount",
            new { controller = "AffiliateAmazon", action = "CreateAccount" },
            new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeCreateAccount);
            routes.Insert(0, routeCreateAccount);

            var routeUpdateAccount = routes.MapRoute(
           "Nop.Plugin.Affiliate.Amazon.UpdateAccount",
           "admin/AffiliateAmazon/UpdateAccount",
           new { controller = "AffiliateAmazon", action = "UpdateAccount" },
           new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeUpdateAccount);
            routes.Insert(0, routeUpdateAccount);

            var routeDeleteAccount = routes.MapRoute(
            "Nop.Plugin.Affiliate.Amazon.DeleteAccount",
            "admin/AffiliateAmazon/DeleteAccount",
            new { controller = "AffiliateAmazon", action = "DeleteAccount" },
            new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeDeleteAccount);
            routes.Insert(0, routeDeleteAccount);

            var routeExtAccount = routes.MapRoute(
           "Nop.Plugin.Affiliate.Amazon.Extenproduct",
           "admin/AffiliateAmazon/Extenproduct",
           new { controller = "AffiliateAmazon", action = "Extenproduct" },
           new[] { "Nop.Plugin.Affiliate.Amazon.Controllers" });
            routes.Remove(routeExtAccount);
            routes.Insert(0, routeExtAccount);
        }
        public int Priority
        {
            get { return 0; }
        }
    }
}
