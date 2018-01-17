using Nop.Plugin.Worldbuy.SimpleMenu.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Worldbuy.SimpleMenu.Data
{
    public partial class WB_SimpleMenuMap : EntityTypeConfiguration<WB_SimpleMenu>
    {
        public WB_SimpleMenuMap()
        {
            this.ToTable("WB_SimpleMenu");
            this.HasKey(x=>x.Id);
            this.Property(x => x.Name);
            this.Property(x => x.WidgetZone);
            this.Property(x => x.IsActived);

        }
    }
}
