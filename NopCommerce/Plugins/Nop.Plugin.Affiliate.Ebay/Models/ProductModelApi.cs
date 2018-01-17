using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.Ebay.Models
{
    public class ProductModelApi
    {
        public List<AdditionalImage> additionalImages { get; set; }
        public string ageGroup { get; set; }
        public string bidCount { get; set; }
        public string brand { get; set; }
        public List<string> buyingOptions { get; set; }
        public string categoryPath { get; set; }
        public string color { get; set; }
        public string condition { get; set; }
        public string conditionId { get; set; }
        public CurrentBidPrice currentBidPrice { get; set; }
        public string description { get; set; }
        public string energyEfficiencyClass { get; set; }
        public string epid { get; set; }
        public List<EstimatedAvailability> estimatedAvailabilities { get; set; }
        public string gender { get; set; }
        public string gtin { get; set; }
        public Image image { get; set; }
        public string itemAffiliateWebUrl { get; set; }
        public string itemEndDate { get; set; }
        public string itemId { get; set; }
        public ItemLocation itemLocation { get; set; }
        public string sellerItemRevision { get; set; }
        public string itemWebUrl { get; set; }
        public List<LocalizedAspect> localizedAspects { get; set; }
        public MarketingPrice marketingPrice { get; set; }
        public string material { get; set; }
        public string mpn { get; set; }
        public string pattern { get; set; }
        public Price price { get; set; }
        public string priceDisplayCondition { get; set; }
        public PrimaryItemGroup primaryItemGroup { get; set; }
        public PrimaryProductReviewRating primaryProductReviewRating { get; set; }
        public string productFicheWebUrl { get; set; }
        public string quantityLimitPerBuyer { get; set; }
        public ReturnTerms returnTerms { get; set; }
        public Seller seller { get; set; }
        public List<ShippingOption> shippingOptions { get; set; }
        public ShipToLocations shipToLocations { get; set; }
        public string shortDescription { get; set; }
        public string size { get; set; }
        public string sizeSystem { get; set; }
        public string sizeType { get; set; }
        public string subtitle { get; set; }
        public List<Tax> taxes { get; set; }
        public string title { get; set; }
        public string topRatedBuyingExperience { get; set; }
        public string uniqueBidderCount { get; set; }
        public List<Warning> warnings { get; set; }
    }

    public class AdditionalImage
    {
        public string height { get; set; }
        public string imageUrl { get; set; }
        public string width { get; set; }
    }

    public class CurrentBidPrice
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class EstimatedAvailability
    {
        public string availabilityThreshold { get; set; }
        public string availabilityThresholdType { get; set; }
        public List<string> deliveryOptions { get; set; }
        public string estimatedAvailabilityStatus { get; set; }
        public string estimatedAvailableQuantity { get; set; }
        public string estimatedSoldQuantity { get; set; }
    }

    public class Image
    {
        public string height { get; set; }
        public string imageUrl { get; set; }
        public string width { get; set; }
    }

    public class ItemLocation
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string county { get; set; }
        public string postalCode { get; set; }
        public string stateOrProvince { get; set; }
    }

    public class LocalizedAspect
    {
        public string name { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class DiscountAmount
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class OriginalPrice
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class MarketingPrice
    {
        public DiscountAmount discountAmount { get; set; }
        public string discountPercentage { get; set; }
        public OriginalPrice originalPrice { get; set; }
    }

    public class Price
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class ItemGroupAdditionalImage
    {
        public string height { get; set; }
        public string imageUrl { get; set; }
        public string width { get; set; }
    }

    public class ItemGroupImage
    {
        public string height { get; set; }
        public string imageUrl { get; set; }
        public string width { get; set; }
    }

    public class PrimaryItemGroup
    {
        public List<ItemGroupAdditionalImage> itemGroupAdditionalImages { get; set; }
        public string itemGroupHref { get; set; }
        public string itemGroupId { get; set; }
        public ItemGroupImage itemGroupImage { get; set; }
        public string itemGroupTitle { get; set; }
        public string itemGroupType { get; set; }
    }

    public class RatingHistogram
    {
        public string count { get; set; }
        public string rating { get; set; }
    }

    public class PrimaryProductReviewRating
    {
        public string averageRating { get; set; }
        public List<RatingHistogram> ratingHistograms { get; set; }
        public string reviewCount { get; set; }
    }

    public class ReturnPeriod
    {
        public string unit { get; set; }
        public string value { get; set; }
    }

    public class ReturnTerms
    {
        public string extendedHolidayReturnsOffered { get; set; }
        public string refundMethod { get; set; }
        public string restockingFeePercentage { get; set; }
        public string returnInstructions { get; set; }
        public string returnMethod { get; set; }
        public ReturnPeriod returnPeriod { get; set; }
        public string returnsAccepted { get; set; }
        public string returnShippingCostPayer { get; set; }
    }

    public class SellerProvidedLegalAddress
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string countryName { get; set; }
        public string county { get; set; }
        public string postalCode { get; set; }
        public string stateOrProvince { get; set; }
    }

    public class VatDetail
    {
        public string issuingCountry { get; set; }
        public string vatId { get; set; }
    }

    public class SellerLegalInfo
    {
        public string email { get; set; }
        public string fax { get; set; }
        public string imprint { get; set; }
        public string legalContactFirstName { get; set; }
        public string legalContactLastName { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string registrationNumber { get; set; }
        public SellerProvidedLegalAddress sellerProvidedLegalAddress { get; set; }
        public string termsOfService { get; set; }
        public List<VatDetail> vatDetails { get; set; }
    }

    public class Seller
    {
        public string feedbackPercentage { get; set; }
        public string feedbackScore { get; set; }
        public string sellerAccountType { get; set; }
        public SellerLegalInfo sellerLegalInfo { get; set; }
        public string username { get; set; }
    }

    public class AdditionalShippingCostPerUnit
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class ShippingCost
    {
        public string convertedFromCurrency { get; set; }
        public string convertedFromValue { get; set; }
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class ShipToLocationUsedForEstimate
    {
        public string country { get; set; }
        public string postalCode { get; set; }
    }

    public class ShippingOption
    {
        public AdditionalShippingCostPerUnit additionalShippingCostPerUnit { get; set; }
        public string cutOffDateUsedForEstimate { get; set; }
        public string maxEstimatedDeliveryDate { get; set; }
        public string minEstimatedDeliveryDate { get; set; }
        public string quantityUsedForEstimate { get; set; }
        public string shippingCarrierCode { get; set; }
        public ShippingCost shippingCost { get; set; }
        public string shippingCostType { get; set; }
        public string shippingServiceCode { get; set; }
        public ShipToLocationUsedForEstimate shipToLocationUsedForEstimate { get; set; }
        public string trademarkSymbol { get; set; }
        public string type { get; set; }
    }

    public class RegionExcluded
    {
        public string regionName { get; set; }
        public string regionType { get; set; }
    }

    public class RegionIncluded
    {
        public string regionName { get; set; }
        public string regionType { get; set; }
    }

    public class ShipToLocations
    {
        public List<RegionExcluded> regionExcluded { get; set; }
        public List<RegionIncluded> regionIncluded { get; set; }
    }

    public class Region
    {
        public string regionName { get; set; }
        public string regionType { get; set; }
    }

    public class TaxJurisdiction
    {
        public Region region { get; set; }
        public string taxJurisdictionId { get; set; }
    }

    public class Tax
    {
        public string includedInPrice { get; set; }
        public string shippingAndHandlingTaxed { get; set; }
        public TaxJurisdiction taxJurisdiction { get; set; }
        public string taxPercentage { get; set; }
        public string taxType { get; set; }
    }

    public class Parameter
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Warning
    {
        public string category { get; set; }
        public string domain { get; set; }
        public string errorId { get; set; }
        public List<string> inputRefIds { get; set; }
        public string longMessage { get; set; }
        public string message { get; set; }
        public List<string> outputRefIds { get; set; }
        public List<Parameter> parameters { get; set; }
        public string subdomain { get; set; }
    }
}
