using Nop.Plugin.Order.Amazon.Domain;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nop.Plugin.Order.Amazon.Models
{
    public class OrderAmazonModel :BaseNopEntityModel
    {
        public OrderAmazonModel()
        {
            Items = new List<OrderItemAmazonModel>();
        }
        public int OrderId { get; set; }
        public string CartId { get; set; }
        public string HMAC { get; set; }
        public string PurchaseURL { get; set; }
        public DateTime CreateOrder { get; set; }
        public IList<OrderItemAmazonModel> Items { get; set; }
        public class OrderItemAmazonModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }
            public string ASIN { get; set; }
            public string CartItemId { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public int OrderId { get; set; }
            public bool Status { get; set; }
        }
    }
}
