using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Nop.Plugin.Affiliate.Amazon.Domain
{
    public partial class CategoryAmazon : BaseEntity, ILocalizedEntity
    {
        public string BrowseNodeID { get; set; }
        public string ParentBrowseNodeID { get; set; }
        public string Name { get; set; }
        public string SearchIndex { get; set; }
        public bool IsCategoryRoot { get; set; }
        public int Level { get; set; }
    }
}
