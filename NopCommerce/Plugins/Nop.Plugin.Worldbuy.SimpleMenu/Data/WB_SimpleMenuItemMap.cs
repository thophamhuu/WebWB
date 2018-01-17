using Nop.Plugin.Worldbuy.SimpleMenu.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Worldbuy.SimpleMenu.Data
{
    public partial class WB_SimpleMenuItemMap : EntityTypeConfiguration<WB_SimpleMenuItem>
    {
        public WB_SimpleMenuItemMap()
        {
            this.ToTable("WB_SimpleMenuItem");
            this.HasKey(x => x.Id);
            this.Property(x => x.MenuID);
            this.Property(x => x.Title);
            this.Property(x => x.Url);
            this.Property(x => x.IconUrlImage);
            this.Property(x => x.Order);
        }
    }
}
