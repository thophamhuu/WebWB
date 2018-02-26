using System;
using System.Xml.Serialization;

namespace Nop.Plugin.Affiliate.Amazon.Models.Response
{
    [XmlRoot("ItemLookupResponse")]
    [Serializable]
    public class ItemLookupResponse
    {
        public Items Items { get; set; }
    }
    [Serializable()]
    public class Items
    {
        public Item Item { get; set; }
    }
    [Serializable()]
    public class Item
    {
        public string ASIN { get; set; }
        public string ParentASIN { get; set; }
        public string DetailPageURL { get; set; }
        [XmlArray("ItemLinks")]
        [XmlArrayItem("ItemLink")]
        public ItemLink[] ItemLinks { get; set; }
        public int SalesRank { get; set; }
        public AmazonImage SmallImage { get; set; }
        public AmazonImage MediumImage { get; set; }
        public AmazonImage LargeImage { get; set; }
        [XmlArray("ImageSets")]
        [XmlArrayItem("ImageSet")]
        public ImageSet[] ImageSets { get; set; }
        public ItemAttributes ItemAttributes { get; set; }
        //[XmlElement("ItemAttributes")]
        ////[XmlIgnore()]
        //public object ItemAttributesJson { get; set; }
        public OfferSummary OfferSummary { get; set; }
        public Offers Offers { get; set; }
        public CustomerReviews CustomerReviews { get; set; }
        public EditorialReviews EditorialReviews { get; set; }
        public SimilarProducts SimilarProducts { get; set; }
        public Variations Variations { get; set; }
        [XmlArray("VariationAttributes")]
        [XmlArrayItem("VariationAttribute")]
        public VariationAttribute[] VariationAttributes { get; set; }
    }
    [Serializable]
    public class Variations
    {
        public VariationDimensions VariationDimensions { get; set; }
        [XmlElement("Item")]
        public Item[] Item { get; set; }
    }
    [Serializable]
    public class VariationDimensions
    {
        [XmlElement("VariationDimension")]
        public string[] VariationDimension { get;set;}
    }
    [Serializable]
    public class VariationAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    [Serializable]
    public class SimilarProducts
    {
        public SimilarProduct SimilarProduct { get; set; }
    }
    [Serializable]
    public class SimilarProduct
    {
        public string ASIN { get; set; }
        public string Title { get; set; }
    }
    [Serializable]
    public class EditorialReviews
    {
        public EditorialReview EditorialReview { get; set; }
    }
    [Serializable]
    public class EditorialReview
    {
        public string Source { get; set; }
        public string Content { get; set; }
        public int IsLinkSuppressed { get; set; }
    }
    [Serializable]
    public class CustomerReviews
    {
        public string IFrameURL { get; set; }
    }
    [Serializable]
    public class Offers
    {
        public int TotalOffers { get; set; }
        public int TotalOfferPages { get; set; }
        public string MoreOffersUrl { get; set; }
        [XmlElement(ElementName = "Offer")]
        public Offer[] Offer { get; set; }
    }
    [Serializable]
    public class Offer
    {
        public Merchant Merchant { get; set; }
        public OfferAttributes OfferAttributes { get; set; }
        public OfferListing OfferListing { get; set; }
        
    }
    [Serializable]
    public class Merchant
    {
        public string Name { get; set; }
    }
    [Serializable]
    public class OfferAttributes
    {
        public string Condition { get; set; }
    }
    [Serializable]
    public class OfferListing
    {
        public string OfferListingId { get; set; }
        public Price Price { get; set; }
        public Price SalePrice { get; set; }
        public string Availability { get; set; }
        public AvailabilityAttributes AvailabilityAttributes { get; set; }
        public int IsEligibleForSuperSaverShipping { get; set; }
        public int IsEligibleForPrime { get; set; }
    }
    [Serializable]
    public class AvailabilityAttributes
    {
        public string AvailabilityType { get; set; }
        public int MinimumHours { get; set; }
        public int MaximumHours { get; set; }
    }
    [Serializable]
    public class OfferSummary
    {
        public Price LowestNewPrice { get; set; }
        public int TotalNew { get; set; }
        public int TotalUsed { get; set; }
        public int TotalCollectible { get; set; }
        public int TotalRefurbished { get; set; }
    }
    [Serializable()]
    public class ItemAttributes
    {
        public string Title { get; set; }
        public string Binding { get; set; }
        public string Manufacturer { get; set; }
        [XmlElement(ElementName = "Feature")]
        public string[] Feature { get; set; }

        public ItemDimensions ItemDimensions { get; set; }
        public string Label { get; set; }
        public Price ListPrice { get; set; }


        //public int IsAutographed { get; set; }
        //public string Brand { get; set; }
        //public string MPN { get; set; }
        //public string PartNumber { get; set; }
        //public string Size { get; set; }
        //public string Color { get; set; }
        //public int PackageQuantity { get; set; }
        //public string ProductGroup { get; set; }
        //public string Studio { get; set; }
        //public string Publisher { get; set; }
        //public PackageDimensions PackageDimensions { get; set; }
    }
    [Serializable]
    public class Price
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string FormattedPrice { get; set; }
    }
    [Serializable]
    public class PackageDimensions
    {
        public Height Height { get; set; }
        public Length Length { get; set; }
        public Weight Weight { get; set; }
        public Width Width { get; set; }
    }
    [Serializable]
    public class ItemDimensions
    {
        public Height Height { get; set; }
        public Length Length { get; set; }
        public Weight Weight { get; set; }
        public Width Width { get; set; }
    }
    [Serializable]
    public class Height
    {
        [XmlAttribute("Units")]
        public string Units { get; set; }
        [XmlElement("Height")]
        public int Value { get; set; }
    }
    public class Length
    {
        [XmlAttribute("Units")]
        public string Units { get; set; }
        [XmlElement("Length")]
        public int Value { get; set; }
    }
    public class Weight
    {
        [XmlAttribute("Units")]
        public string Units { get; set; }
        [XmlElement("Weight")]
        public int Value { get; set; }
    }
    public class Width
    {
        [XmlAttribute("Units")]
        public string Units { get; set; }
        [XmlElement("Width")]
        public int Value { get; set; }
    }
    [Serializable()]
    public class ItemLink
    {
        public string Description { get; set; }
        public string URL { get; set; }
    }
    [Serializable()]
    public class AmazonImage
    {
        public string URL { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    [Serializable()]
    public class ImageSet
    {
        [XmlAttribute("Category")]
        public string Category { get; set; }
        public AmazonImage SwatchImage { get; set; }
        public AmazonImage SmallImage { get; set; }
        public AmazonImage ThumbnailImage { get; set; }
        public AmazonImage TinyImage { get; set; }
        public AmazonImage MediumImage { get; set; }
        public AmazonImage LargeImage { get; set; }
        public AmazonImage HiResImage { get; set; }
    }
}
