using Nop.Plugin.Affiliate.Ebay.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.Ebay.Data
{
    public class CategoryEbayMap : EntityTypeConfiguration<CategoryEbay>
    {
        public CategoryEbayMap()
        {
            ToTable("CategoryEbay");
            HasKey(m => m.Id);

            Property(m => m.EbayId);
            Property(m => m.Name);
        }
    }
}
