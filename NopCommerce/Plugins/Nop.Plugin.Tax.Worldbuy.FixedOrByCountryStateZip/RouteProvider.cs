using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.AddRateByCountryStateZip",
                 "Plugins/Worldbuy/FixedOrByCountryStateZip/AddRateByCountryStateZip",
                 new { controller = "WB_FixedOrByCountryStateZip", action = "AddRateByCountryStateZip" },
                 new[] { "Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Controllers" }
            );
            routes.MapRoute("Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.AddTaxCategoryMapping",
                 "Plugins/Worldbuy/FixedOrByCountryStateZip/AddTaxCategoryMapping",
                 new { controller = "WB_FixedOrByCountryStateZip", action = "AddTaxCategoryMapping" },
                 new[] { "Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Controllers" }
            );
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