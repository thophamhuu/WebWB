using Nop.Core;

namespace Nop.Plugin.Shipping.RateByDistrict.Domain
{
    public class ShippingByCategoryRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }
        public int ProductTypeId { get; set; }
        /// Gets or sets the CategoryId
        /// </summary>
        public int CategoryId { get; set; }
        public decimal AdditionalFixedCost { get; set; }
    }
}
