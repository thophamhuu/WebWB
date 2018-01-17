using Nop.Core;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Domain
{
    /// <summary>
    /// Represents a tax rate
    /// </summary>
    public partial class WB_TaxCategoryMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the tax category identifier
        /// </summary>
        public int TaxCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the tax category identifier
        /// </summary>
        public int CategoryId { get; set; }
        public decimal Percentage{ get; set; }
    }
}