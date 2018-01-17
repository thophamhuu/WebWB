using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Shipping.RateByDistrict.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.ShippingByDistrictEnabled")]

        public bool ShippingByDistrictEnabled { get; set; }
    }
}