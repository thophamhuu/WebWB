using Nop.Plugin.Worldbuy.AnyBanner.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Worldbuy.AnyBanner.Data
{
    public partial class WB_AnyBannerItemMap : EntityTypeConfiguration<WB_AnyBannerItem>
    {
        public WB_AnyBannerItemMap()
        {
            this.ToTable("WB_AnyBannerItem");
            this.HasKey(x => x.Id);
        }
    }
}
