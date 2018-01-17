using Nop.Data.Mapping;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Domain;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Data
{
    public partial class WB_TaxRateMap : NopEntityTypeConfiguration<WB_TaxRate>
    {
        public WB_TaxRateMap()
        {
            this.ToTable("WB_TaxRate");
            this.HasKey(tr => tr.Id);
            this.Property(tr => tr.Percentage).HasPrecision(18, 4);
        }
    }
}