using Nop.Core;

namespace Nop.Plugin.Shipping.RateByDistrict.Domain
{
    public class ShippingByDistrictRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }
        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state/province identifier
        /// </summary>
        public int StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the zip
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// Gets or sets the shipping method identifier
        /// </summary>
        public int ShippingMethodId { get; set; }

        /// <summary>
        /// Gets or sets the additional fixed cost
        /// </summary>
        public decimal AdditionalFixedCost { get; set; }
    }
}
