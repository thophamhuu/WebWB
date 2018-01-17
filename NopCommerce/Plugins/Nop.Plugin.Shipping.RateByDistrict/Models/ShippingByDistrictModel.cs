using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Shipping.RateByDistrict.Models
{
    public class ShippingByDistrictModel : BaseNopEntityModel
    {
        public ShippingByDistrictModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableShippingMethods = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.Store")]
        public int StoreId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.Store")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.Country")]
        public int CountryId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.Country")]
        public string CountryName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.StateProvince")]
        public string StateProvinceName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.Zip")]
        public string Zip { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.ShippingMethod")]
        public int ShippingMethodId { get; set; }
        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.ShippingMethod")]
        public string ShippingMethodName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.AdditionalFixedCost")]
        public decimal AdditionalFixedCost { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.RateByDistrict.Fields.DataHtml")]
        public string DataHtml { get; set; }
        public string PrimaryStoreCurrencyCode { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
        public IList<SelectListItem> AvailableShippingMethods { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}