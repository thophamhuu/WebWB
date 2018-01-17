using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Shipping.RateByDistrict.Models
{
    public class ShippingByProductTypeModel : BaseNopEntityModel
    {
        public ShippingByProductTypeModel()
        {
        }

        [NopResourceDisplayName("Plugins.Shipping.RateByDistric.ProductTypet.Fields.Store")]
        public int StoreId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.ProductType.Fields.Store")]
        public string StoreName { get; set; }
        /// <summary>
        /// Gets or sets the Product Type Name
        /// </summary>
        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.ProductType.Fields.ProductTypeName")]
        public string ProductTypeName { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.ProductType.Fields.AdditionalFixedCost")]
        public decimal AdditionalFixedCost { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; } = new List<SelectListItem>();
        public string PrimaryStoreCurrencyCode { get; set; }
    }
}