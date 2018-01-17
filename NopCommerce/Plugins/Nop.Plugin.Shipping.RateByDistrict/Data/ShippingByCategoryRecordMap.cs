using Nop.Data.Mapping;
using Nop.Plugin.Shipping.RateByDistrict.Domain;

namespace Nop.Plugin.Shipping.RateByDistrict.Data
{
    public class ShippingByCategoryRecordMap : NopEntityTypeConfiguration<ShippingByCategoryRecord>
    {
        public ShippingByCategoryRecordMap()
        {
            this.ToTable("ShippingByCategory");
            this.HasKey(x => x.Id);
        }
    }
}
