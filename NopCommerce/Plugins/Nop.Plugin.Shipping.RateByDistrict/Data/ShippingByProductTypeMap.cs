using Nop.Data.Mapping;
using Nop.Plugin.Shipping.RateByDistrict.Domain;

namespace Nop.Plugin.Shipping.RateByDistrict.Data
{
    public class ShippingByProductTypeRecordMap : NopEntityTypeConfiguration<ShippingByProductTypeRecord>
    {
        public ShippingByProductTypeRecordMap()
        {
            this.ToTable("ShippingProductType");
            this.HasKey(x => x.Id);
        }
    }
}
