using Nop.Plugin.Worldbuy.StateProvinceWB.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Worldbuy.StateProvinceWB.Data
{
    public partial class StateProvincePostalCodeMap : EntityTypeConfiguration<StateProvincePostalCode>
    {
        public StateProvincePostalCodeMap()
        {
            this.ToTable("StateProvincePostalCode");
            this.HasKey(x=>x.Id);
            this.Property(x => x.StateProvinceID);
            this.Property(x => x.PostalCode).HasMaxLength(100);
        }
    }
}
