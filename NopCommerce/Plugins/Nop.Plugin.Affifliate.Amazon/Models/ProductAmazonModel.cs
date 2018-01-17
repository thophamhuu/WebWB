namespace Nop.Plugin.Affiliate.Amazon.Models
{
    public class ProductAmazonModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ASIN { get; set; }
        public string DetailUrl { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string PriceSource { get; set; }
    }
}