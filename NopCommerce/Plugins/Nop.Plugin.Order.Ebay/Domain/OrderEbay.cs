using Nop.Core;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Ebay.Domain
{
    public class OrderEbay : BaseEntity, ILocalizedEntity
    {
        public string OrderId { get; set; }
        public string CreationDate { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public string RefundedAmount { get; set; }

        public string Adjustment { get; set; }
        public string DeliveryCost { get; set; }
        public string DeliveryDiscount { get; set; }
        public string Fee { get; set; }
        public string PriceDiscount { get; set; }
        public string PriceSubtotal { get; set; }
        public string Tax { get; set; }
        public string Total { get; set; }
    }
}
