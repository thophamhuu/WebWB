using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.RateByDistrict
{
    public class RateByDistrictSettings : ISettings
    {
        public bool ShippingByDistrictEnabled { get; set; }
    }
}