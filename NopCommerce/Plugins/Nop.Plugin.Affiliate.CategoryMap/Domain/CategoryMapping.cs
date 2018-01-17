using Nop.Core;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.CategoryMap.Domain
{
    public class CategoryMapping : BaseEntity, ILocalizedEntity
    {
        public int CategoryId { get; set; }
        public int CategorySourceId { get; set; }
        public int SourceId { get; set; }
    }
}
