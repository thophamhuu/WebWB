using Nop.Plugin.Order.Amazon.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nop.Plugin.Order.Amazon.Models
{
    public class OrderModel : Nop.Admin.Models.Orders.OrderModel
    {
        public OrderModel() : base()
        {
        }
        public bool HasCart { get {
                return OrderAmazon != null;
            } }
        public OrderAmazonModel OrderAmazon { get; set; }
        public Cart Cart { get; set; }
    }
}
