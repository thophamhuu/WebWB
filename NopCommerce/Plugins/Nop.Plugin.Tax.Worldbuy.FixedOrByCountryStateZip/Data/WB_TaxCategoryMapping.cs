using Nop.Data.Mapping;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Domain;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Data
{
    public partial class WB_TaxCategoryMappingMap : NopEntityTypeConfiguration<WB_TaxCategoryMapping>
    {
        public WB_TaxCategoryMappingMap()
        {
            this.ToTable("WB_TaxCategoryMapping");
            this.HasKey(tr => tr.Id);
            this.Property(tr => tr.Percentage).HasPrecision(18, 4);
        }
    }
}