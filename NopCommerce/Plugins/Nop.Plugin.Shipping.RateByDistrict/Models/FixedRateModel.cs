using Nop.Web.Framework;

namespace Nop.Plugin.Shipping.RateByDistrict.Models
{
    public class FixedRateModel
    {
        public int ShippingMethodId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.ShippingMethod")]
        public string ShippingMethodName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.Rate")]
        public decimal Rate { get; set; }
    }
}