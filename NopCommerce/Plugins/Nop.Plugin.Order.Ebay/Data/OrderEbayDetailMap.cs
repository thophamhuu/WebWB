using Nop.Plugin.Order.Ebay.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Ebay.Data
{
    public class OrderEbayDetailMap : EntityTypeConfiguration<OrderEbayDetail>
    {
        public OrderEbayDetailMap()
        {
            ToTable("OrderEbayDetail");
            HasKey(m => m.Id);

            Property(m => m.OrderId);
            Property(m => m.ItemId);
            Property(m => m.LineItemId);           
            Property(m => m.Title);
            Property(m => m.Quantity);
            Property(m => m.NetPrice);
            Property(m => m.LineItemPaymentStatus);
            Property(m => m.LineItemStatus);
            Property(m => m.ShippingCarrierCode);
            Property(m => m.ShippingServiceCode);
            Property(m => m.MinEstimatedDeliveryDate);
            Property(m => m.MaxEstimatedDeliveryDate);
            Property(m => m.FeedbackPercentage);
            Property(m => m.FeedbackScore);
            Property(m => m.SellerAccountType);
            Property(m => m.Username);
        }
    }
}
