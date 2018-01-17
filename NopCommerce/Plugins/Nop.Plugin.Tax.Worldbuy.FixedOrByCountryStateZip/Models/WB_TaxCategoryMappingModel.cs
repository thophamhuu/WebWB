using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Models
{
    public class WB_TaxCategoryMappingModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Store")]
        public int StoreId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Store")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategory")]
        public int TaxCategoryId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.TaxCategory")]
        public string TaxCategoryName { get; set; }

        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Category")]
        public int CategoryId { get; set; }
        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Category")]
        public string CategoryName { get; set; }


        [NopResourceDisplayName("Plugins.Tax.Worldbuy.FixedOrByCountryStateZip.Fields.Percentage")]
        public decimal Percentage { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableTaxCategories { get; set; }
    }
}