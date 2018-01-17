using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Models
{
    public class WB_CountryStateZipModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Store")]
        public int StoreId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Store")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategory")]
        public int TaxCategoryId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategory")]
        public string TaxCategoryName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Country")]
        public int CountryId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Country")]
        
public string CountryName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.StateProvince")]
        public string StateProvinceName { get; set; }
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Zip")]
        public string Zip { get; set; }
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Percentage")]
        public decimal Percentage { get; set; }
    }
}