using Nop.Core;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.Ebay.Domain
{
    public class CategoryEbay : BaseEntity, ILocalizedEntity
    {
        public int EbayId { get; set; }
        public string Name { get; set; }
        public int ParentCategoryId { get; set; }
        public int Level { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
    }
}
