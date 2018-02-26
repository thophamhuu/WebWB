using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Nop.Plugin.Affiliate.Amazon.Models.Response
{
    [XmlRoot("BrowseNodeLookupResponse")]
    [Serializable]
    public class BrowseNodeLookupResponse
    {
        [XmlElement("BrowseNodes")]
        public BrowseNodes BrowseNodes { get; set; }
    }
    [Serializable()]
    public class BrowseNodes
    {
        public BrowseNode BrowseNode { get; set; }
    }
    [Serializable()]
    public class BrowseNode
    {
        [XmlElement("BrowseNodeId")]
        public string BrowseNodeId { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("IsCategoryRoot")]
        public int IsCategoryRoot { get; set; }
        [XmlArray("Children")]
        [XmlArrayItem("BrowseNode")]
        public BrowseNode[] Children { get; set; }
    }
   

    [XmlRoot("BrowseNodeLookupErrorResponse")]
    [Serializable]
    public class BrowseNodeLookupErrorResponse : ErrorResponse
    {
        public string RequestID { get; set; }
    }
    public class ErrorResponse
    {
        public Error Error { get; set; }
    }
    [Serializable]
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
