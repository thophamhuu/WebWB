namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Infrastructure.Cache
{
    /// <summary>
    /// Represents a tax rate
    /// </summary>
    public partial class WB_TaxCategoryMappingForCaching
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int TaxCategoryId { get; set; }
        public int CategoryId { get; set; }
        public decimal Percentage { get; set; }
    }
}