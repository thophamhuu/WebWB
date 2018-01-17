using Nop.Core;

namespace Nop.Plugin.Shipping.RateByDistrict.Domain
{
    public class ShippingByProductTypeRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }
        /// Gets or sets the ProductTypeName
        /// </summary>
        public string ProductTypeName { get; set; }
        public decimal AdditionalFixedCost { get; set; }
    }
}
