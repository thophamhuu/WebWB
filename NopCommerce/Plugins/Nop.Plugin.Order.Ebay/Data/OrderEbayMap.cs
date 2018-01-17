using Nop.Plugin.Order.Ebay.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Ebay.Data
{
    public class OrderEbayMap : EntityTypeConfiguration<OrderEbay>
    {
        public OrderEbayMap()
        {
            ToTable("OrderEbay");
            HasKey(m => m.Id);

            Property(m => m.OrderId);
            Property(m => m.CreationDate);
            Property(m => m.PaymentStatus);
            Property(m => m.OrderStatus);
            Property(m => m.RefundedAmount);
            Property(m => m.Adjustment);
            Property(m => m.DeliveryCost);
            Property(m => m.DeliveryDiscount);
            Property(m => m.Fee);
            Property(m => m.PriceDiscount);
            Property(m => m.PriceSubtotal);
            Property(m => m.Tax);
            Property(m => m.Total);
        }
    }
}
