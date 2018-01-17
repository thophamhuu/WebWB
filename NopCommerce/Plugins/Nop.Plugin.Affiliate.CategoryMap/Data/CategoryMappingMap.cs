using Nop.Plugin.Affiliate.CategoryMap.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.CategoryMap.Data
{
    public partial class CategoryMappingMap : EntityTypeConfiguration<CategoryMapping>
    {
        public CategoryMappingMap()
        {
            this.ToTable("CategoryMapping");
            this.HasKey(c => c.Id);
            this.Property(c => c.CategoryId);
            this.Property(c => c.CategorySourceId);
            this.Property(c => c.SourceId);
        }
    }
}
