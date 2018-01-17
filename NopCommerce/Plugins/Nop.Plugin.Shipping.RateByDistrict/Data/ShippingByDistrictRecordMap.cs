using Nop.Data.Mapping;
using Nop.Plugin.Shipping.RateByDistrict.Domain;

namespace Nop.Plugin.Shipping.RateByDistrict.Data
{
    public class ShippingByDistrictRecordMap : NopEntityTypeConfiguration<ShippingByDistrictRecord>
    {
        public ShippingByDistrictRecordMap()
        {
            this.ToTable("ShippingByDistrict");
            this.HasKey(x => x.Id);

            this.Property(x => x.Zip).HasMaxLength(400);
        }
    }
}
