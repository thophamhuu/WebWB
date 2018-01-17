using Nop.Web.Framework;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Models
{
    public class WB_FixedTaxRateModel
    {
        public int TaxCategoryId { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategoryName")]
        public string TaxCategoryName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Rate")]
        public decimal Rate { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.IsAbsolute")]
        public bool IsAbsolute { get; set; }
    }
}