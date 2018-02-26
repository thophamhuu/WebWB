using System;
using System.Xml.Serialization;

namespace Nop.Plugin.Affiliate.Amazon.Models.Response
{
    [XmlRoot("ItemSearchResponse")]
    [Serializable]
    public class ItemSearchResponse
    {
        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public Item[] Items { get; set; }
    }
    
}
