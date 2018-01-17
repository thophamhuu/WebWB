using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Affiliate.CategoryMap.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
        [NopResourceDisplayName("Phần trăm")]
        public decimal AdditionalCostPercent { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Affiliate.ShippingDescriptions")]
        public string ShippingDescriptions { get; set; }
    }
}