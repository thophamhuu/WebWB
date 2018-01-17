using Nop.Plugin.Affiliate.CategoryMap.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.CategoryMap.Data
{
    public partial class ProductMappingMap : EntityTypeConfiguration<ProductMapping>
    {
        public ProductMappingMap()
        {
            this.ToTable("ProductMapping");
            this.HasKey(c => c.Id);
            this.Property(c => c.ProductId);
            this.Property(c => c.ProductSourceId);
            this.Property(c => c.ProductSourceLink);
            this.Property(c => c.SourceId);
            this.Property(c => c.Price);
            this.Ignore(c => c.VariationsXML);
            this.Ignore(c => c.Variations);
        }
    }
}
