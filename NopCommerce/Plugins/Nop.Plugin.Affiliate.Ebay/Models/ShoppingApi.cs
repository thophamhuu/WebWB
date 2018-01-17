using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.Ebay.Models
{
    public class ShoppingApi
    {
        public string Timestamp { get; set; }
        public string Ack { get; set; }
        public List<Error> Errors { get; set; }
        public string Build { get; set; }
        public string Version { get; set; }
        public Item Item { get; set; }
    }

    public class Error
    {
        public string ShortMessage { get; set; }
        public string LongMessage { get; set; }
        public string ErrorCode { get; set; }
        public string SeverityCode { get; set; }
        public string ErrorClassification { get; set; }
    }

    public class ConvertedCurrentPrice
    {
        public double Value { get; set; }
        public string CurrencyID { get; set; }
    }

    public class DiscountPriceInfo
    {
        public string PricingTreatment { get; set; }
    }

    public class Item
    {
        public string ItemID { get; set; }
        public string EndTime { get; set; }
        public string ViewItemURLForNaturalSearch { get; set; }
        public string ListingType { get; set; }
        public string Location { get; set; }
        public List<string> PictureURL { get; set; }
        public string PrimaryCategoryID { get; set; }
        public string PrimaryCategoryName { get; set; }
        public int BidCount { get; set; }
        public ConvertedCurrentPrice ConvertedCurrentPrice { get; set; }
        public string ListingStatus { get; set; }
        public string TimeLeft { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }
        public bool AutoPay { get; set; }
        public int ConditionID { get; set; }
        public string ConditionDisplayName { get; set; }
        public string QuantityAvailableHint { get; set; }
        public DiscountPriceInfo DiscountPriceInfo { get; set; }
        public string ConditionDescription { get; set; }
    }
}
