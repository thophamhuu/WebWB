using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Shipping.RateByDistrict
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Shipping.RateByDistrict.Configure",
                 "Plugins/RateByDistrict/Configure",
                 new { controller = "RateByDistrict", action = "Configure", },
                 new[] { "Nop.Plugin.Shipping.RateByDistrict.Controllers" }
            );

            routes.MapRoute("Plugin.Shipping.RateByDistrict.AddRateByDistrictPopup",
                 "Plugins/RateByDistrict/AddRateByDistrictPopup",
                 new { controller = "RateByDistrict", action = "AddRateByDistrictPopup" },
                 new[] { "Nop.Plugin.Shipping.RateByDistrict.Controllers" }
            );

            routes.MapRoute("Plugin.Shipping.RateByDistrict.EditRateByDistrictPopup",
                 "Plugins/RateByDistrict/EditRateByDistrictPopup",
                 new { controller = "RateByDistrict", action = "EditRateByDistrictPopup" },
                 new[] { "Nop.Plugin.Shipping.RateByDistrict.Controllers" }
            );

            var routeTypes = routes.MapRoute("Plugin.Shipping.RateByDistrict.ProductTypes",
             "Admin/RateByDistrict/ProductTypes",
             new { controller = "RateByDistrict", action = "ProductTypes", },
             new[] { "Nop.Plugin.Shipping.RateByDistrict.Controllers" }

            );
            routes.Remove(routeTypes);
            routes.Insert(0, routeTypes);

            var routeType = routes.MapRoute("Plugin.Shipping.RateByDistrict.ProductType",
            "Admin/RateByDistrict/ProductType",
            new { controller = "RateByDistrict", action = "ProductType", },
            new[] { "Nop.Plugin.Shipping.RateByDistrict.Controllers" }

           );
            routes.Remove(routeType);
            routes.Insert(0, routeType);

            var routeDeleteType = routes.MapRoute("Plugin.Shipping.RateByDistrict.DeleteProductType",
            "Admin/RateByDistrict/DeleteProductType",
            new { controller = "RateByDistrict", action = "DeleteProductType", },
            new[] { "Nop.Plugin.Shipping.RateByDistrict.Controllers" }

           );
            routes.Remove(routeDeleteType);
            routes.Insert(0, routeDeleteType);

            var routeCategories = routes.MapRoute("Plugin.Shipping.RateByDistrict.Categories",
            "Admin/RateByDistrict/Categories",
            new { controller = "RateByDistrict", action = "Categories", },
            new[] { "Nop.Plugin.Shipping.RateByDistrict.Controllers" }

           );
            routes.Remove(routeCategories);
            routes.Insert(0, routeCategories);

            var routeCategory = routes.MapRoute("Plugin.Shipping.RateByDistrict.Category",
            "Admin/RateByDistrict/Category",
            new { controller = "RateByDistrict", action = "Category", },
            new[] { "Nop.Plugin.Shipping.RateByDistrict.Controllers" }

           );
            routes.Remove(routeCategory);
            routes.Insert(0, routeCategory);

            var routeDeleteCategory = routes.MapRoute("Plugin.Shipping.RateByDistrict.DeleteCategory",
           "Admin/RateByDistrict/DeleteCategory",
           new { controller = "RateByDistrict", action = "DeleteCategory", },
           new[] { "Nop.Plugin.Shipping.RateByDistrict.Controllers" }

          );
            routes.Remove(routeDeleteCategory);
            routes.Insert(0, routeDeleteCategory);
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
