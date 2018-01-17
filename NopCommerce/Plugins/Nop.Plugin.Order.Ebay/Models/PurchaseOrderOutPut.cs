using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Ebay.Models
{
    public class PurchaseOrderOutPut
    {
        public List<LineItem1> lineItems { get; set; }
        public PricingSummary pricingSummary { get; set; }
        public string purchaseOrderCreationDate { get; set; }
        public string purchaseOrderId { get; set; }
        public string purchaseOrderPaymentStatus { get; set; }
        public string purchaseOrderStatus { get; set; }
        public RefundedAmount refundedAmount { get; set; }
        public List<Warning> warnings { get; set; }
    }

    public class ShippingDetail
    {
        public string maxEstimatedDeliveryDate { get; set; }
        public string minEstimatedDeliveryDate { get; set; }
        public string shippingCarrierCode { get; set; }
        public string shippingServiceCode { get; set; }
    }

    public class LineItem1
    {
        public Image image { get; set; }
        public string itemId { get; set; }
        public string lineItemId { get; set; }
        public string lineItemPaymentStatus { get; set; }
        public string lineItemStatus { get; set; }
        public NetPrice netPrice { get; set; }
        public string quantity { get; set; }
        public Seller seller { get; set; }
        public ShippingDetail shippingDetail { get; set; }
        public string title { get; set; }
    }


    public class RefundedAmount
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

}
