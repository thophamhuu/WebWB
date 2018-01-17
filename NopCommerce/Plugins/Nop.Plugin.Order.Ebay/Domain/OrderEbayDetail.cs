using Nop.Core;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Ebay.Domain
{
    public class OrderEbayDetail : BaseEntity, ILocalizedEntity
    {
        public int OrderId { get; set; }
        public string ItemId { get; set; }
        public string LineItemId { get; set; }
        public string LineItemPaymentStatus { get; set; }
        public string LineItemStatus { get; set; }
        public string Title { get; set; }
        public string Quantity { get; set; }
        public string NetPrice { get; set; }
        public string ShippingCarrierCode { get; set; }
        public string ShippingServiceCode { get; set; }
        public string MinEstimatedDeliveryDate { get; set; }
        public string MaxEstimatedDeliveryDate { get; set; }
        public string FeedbackPercentage { get; set; }
        public int FeedbackScore { get; set; }
        public string SellerAccountType { get; set; }
        public string Username { get; set; }

    }
}
