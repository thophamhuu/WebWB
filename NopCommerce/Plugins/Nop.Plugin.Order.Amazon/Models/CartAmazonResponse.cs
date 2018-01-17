using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nop.Plugin.Order.Amazon.Models
{
    [XmlRoot("CartCreateResponse")]
    [Serializable]
    public class CartCreateResponse
    {
        [XmlElement("Cart")]
        public Cart Cart { get; set; }
    }
    [XmlRoot("CartGetResponse")]
    [Serializable]
    public class CartGetResponse
    {
        [XmlElement("Cart")]
        public Cart Cart { get; set; }
    }
    [XmlRoot("CartAddResponse")]
    [Serializable]
    public class CartAddResponse
    {
        [XmlElement("Cart")]
        public Cart Cart { get; set; }
    }
    [XmlRoot("CartModifyResponse")]
    [Serializable]
    public class CartModifyResponse
    {
        [XmlElement("Cart")]
        public Cart Cart { get; set; }
    }
    [XmlRoot("CartClearResponse")]
    [Serializable]
    public class CartClearResponse
    {
        [XmlElement("Cart")]
        public Cart Cart { get; set; }
    }

    [Serializable()]
    public class Cart
    {
        [XmlElement("CartId")]
        public string CartId { get; set; }
        [XmlElement("HMAC")]
        public string HMAC { get; set; }
        [XmlElement("PurchaseURL")]
        public string PurchaseURL { get; set; }
        [XmlElement("SubTotal")]
        public Price SubTotal { get; set; }
        [XmlArray("CartItems")]
        [XmlArrayItem("CartItem")]
        public CartItem[] CartItems { get; set; }
    }
    #region Request
    [Serializable()]
    public class Item
    {
        [XmlElement("CartItemId")]
        public string CartItemId { get; set; }
        [XmlElement("ASIN")]
        public string ASIN { get; set; }
        [XmlElement("OfferListingId")]
        public string OfferListingId { get; set; }
        [XmlElement("Quantity")]
        public int Quantity { get; set; }
    }
    #endregion
    [Serializable()]
    public class CartItem
    {
        [XmlElement("CartItemId")]
        public string CartItemId { get; set; }
        [XmlElement("ASIN")]
        public string ASIN { get; set; }
        [XmlElement("Quantity")]
        public int Quantity { get; set; }
        [XmlElement("Title")]
        public string Title { get; set; }
        [XmlElement("ProductGroup")]
        public string ProductGroup { get; set; }
        [XmlElement("Price")]
        public Price Price { get; set; }
        [XmlElement("ItemTotal")]
        public Price ItemTotal { get; set; }
        public int OrderAmazonId { get; set; }
    }
    [Serializable()]
    public class Price
    {
        [XmlElement("Amount")]
        public decimal Amount { get; set; }
        [XmlElement("CurrencyCode")]
        public string CurrencyCode { get; set; }
        [XmlElement("FormattedPrice")]
        public string FormattedPrice { get; set; }

        public decimal PriceValue { get; set; }
    }
}
