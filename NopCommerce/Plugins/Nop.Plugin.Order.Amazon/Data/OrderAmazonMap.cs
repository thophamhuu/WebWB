using Nop.Plugin.Order.Amazon.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.Order.Amazon.Data
{
    public partial class OrderAmazonMap : EntityTypeConfiguration<OrderAmazon>
    {
        public OrderAmazonMap()
        {
            this.ToTable("OrderAmazon");
            this.HasKey(c => c.Id);
            this.Property(c => c.CartId).IsRequired();
            this.Property(c => c.CreateOrder).IsRequired();
            this.Property(c => c.HMAC).IsRequired();
            this.Property(c => c.OrderId).IsRequired();
            this.Property(c => c.PurchaseURL).IsRequired();
        }
    }
}
