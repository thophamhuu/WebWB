using System.Collections.Generic;

namespace Nop.Plugin.Affiliate.Amazon.Models
{
    public class BrowseNodeModel
    {
        public string BrowseNodeID { get; set; }
        public string Name { get; set; }
        public bool IsCategoryRoot { get; set; }
        public string MapCategory { get; set; }
        public int Level { get; set; }
        public IList<BrowseNodeModel> Children { get; set; }
    }
}
