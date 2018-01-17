using Nop.Core.Configuration;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip
{
    public class WB_FixedOrByCountryStateZipTaxSettings : ISettings
    {
        public bool CountryStateZipEnabled { get; set; }
        public bool IsAbsolute { get; set; }
    }
}