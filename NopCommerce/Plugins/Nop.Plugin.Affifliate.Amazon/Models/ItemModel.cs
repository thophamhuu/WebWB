using System.Collections.Generic;

namespace Nop.Plugin.Affiliate.Amazon.Models
{
    public class ItemModel
    {
        public string ASIN { get; set; }
        public string ParentASIN { get; set; }
        public string DetailUrl { get; set; }
        public IList<string> Images { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string CurrenceCode { get; set; }
        public string Short_Descriptions { get; set; }
        public string Full_Descriptions { get; set; }
    }
}