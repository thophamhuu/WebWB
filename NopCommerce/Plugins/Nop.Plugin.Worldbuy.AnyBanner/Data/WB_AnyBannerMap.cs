using Nop.Plugin.Worldbuy.AnyBanner.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Worldbuy.AnyBanner.Data
{
    public partial class WB_AnyBannerMap : EntityTypeConfiguration<WB_AnyBanner>
    {
        public WB_AnyBannerMap()
        {
            this.ToTable("WB_AnyBanner");
            this.HasKey(x => x.Id);
            this.Property(x => x.Name);
            this.Property(x => x.IsActived);
            this.Property(x => x.WidgetZone);
        }
    }
}
