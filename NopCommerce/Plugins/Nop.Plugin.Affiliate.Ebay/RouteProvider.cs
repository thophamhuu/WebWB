using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Affiliate.Ebay
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var routeConfigure = routes.MapRoute("Plugin.Affiliate.Ebay.Configure",
                 "Admin/AffiliateEbay/Configure",
                 new { controller = "AffiliateEbay", action = "Configure" },
                 new[] { "Nop.Plugin.Affiliate.Ebay.Controllers" }
            );
            routes.Remove(routeConfigure);
            routes.Insert(0, routeConfigure);

            var routeCallApi = routes.MapRoute("Plugin.Affiliate.Ebay.CallApi",
                "Admin/AffiliateEbay/CallApi",
                new { controller = "AffiliateEbay", action = "CallApi" },
                new[] { "Nop.Plugin.Affiliate.Ebay.Controllers" }
           );
            routes.Remove(routeCallApi);
            routes.Insert(0, routeCallApi);

            var routeMapCategory = routes.MapRoute("Plugin.Affiliate.Ebay.MapCategory",
                "Admin/AffiliateEbay/MapCategory",
                new { controller = "AffiliateEbay", action = "MapCategory" },
                new[] { "Nop.Plugin.Affiliate.Ebay.Controllers" }
           );
            routes.Remove(routeMapCategory);
            routes.Insert(0, routeMapCategory);

            var test = routes.MapRoute("Plugin.Affiliate.Ebay.ConvertPrice",
                "Admin/AffiliateEbay/ConvertPrice",
                new { controller = "AffiliateEbay", action = "ConvertPrice" },
                new[] { "Nop.Plugin.Affiliate.Ebay.Controllers" }
           );
            routes.Remove(test);
            routes.Insert(0, test);
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
