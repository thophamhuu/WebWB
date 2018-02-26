using Nop.Plugin.Affiliate.Amazon.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Affiliate.Amazon.Data
{
    public partial class CategoryAmazonMap : EntityTypeConfiguration<CategoryAmazon>
    {
        public CategoryAmazonMap()
        {
            this.ToTable("CategoryAmazon");
            this.HasKey(c => c.Id);
            this.Property(c => c.BrowseNodeID).IsRequired().HasMaxLength(20);
            this.Property(c => c.ParentBrowseNodeID).HasMaxLength(20);
            this.Property(c => c.Name).IsRequired().HasMaxLength(200);
            this.Property(c => c.SearchIndex).HasMaxLength(200);
            this.Property(c => c.Level);
            this.Property(c => c.IsCategoryRoot).IsRequired();
        }
    }
}
