using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Affiliate.Amazon.Models
{
    public partial class CategoryAmazonModel: BaseNopEntityModel
    {
        public string BrowseNodeID { get; set; }
        public string ParentBrowseNodeID { get; set; }
        public string Name { get; set; }
        public string SearchIndex { get; set; }
        public bool IsCategoryRoot { get; set; }
        public int CategoryMapID { get; set; } = 0;
        public int CategoryID { get; set; } = 0;
        public string CategoryName { get; set; }

        public int Level { get; set; }
    }
}
