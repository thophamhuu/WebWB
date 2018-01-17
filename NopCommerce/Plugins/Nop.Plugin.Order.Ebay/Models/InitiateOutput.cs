using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Ebay.Models
{
    public class InitiateOutput
    {
        public List<AcceptedPaymentMethod> acceptedPaymentMethods { get; set; }
        public string checkoutSessionId { get; set; }
        public string expirationDate { get; set; }
        public List<LineItem> lineItems { get; set; }
        public PricingSummary pricingSummary { get; set; }
        public ProvidedPaymentInstrument providedPaymentInstrument { get; set; }
        public ShippingAddress shippingAddress { get; set; }
        public List<Warning> warnings { get; set; }
    }

    public class LogoImage
    {
        public string height { get; set; }
        public string imageUrl { get; set; }
        public string width { get; set; }
    }

    public class LogoImage2
    {
        public string height { get; set; }
        public string imageUrl { get; set; }
        public string width { get; set; }
    }

    public class PaymentMethodBrand
    {
        public LogoImage2 logoImage { get; set; }
        public string paymentMethodBrandType { get; set; }
    }

    public class PaymentMethodMessage
    {
        public string legalMessage { get; set; }
        public string requiredForUserConfirmation { get; set; }
    }

    public class AcceptedPaymentMethod
    {
        public string label { get; set; }
        public LogoImage logoImage { get; set; }
        public List<PaymentMethodBrand> paymentMethodBrands { get; set; }
        public List<PaymentMethodMessage> paymentMethodMessages { get; set; }
        public string paymentMethodType { get; set; }
    }

    public class BaseUnitPrice
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Image
    {
        public string height { get; set; }
        public string imageUrl { get; set; }
        public string width { get; set; }
    }

    public class NetPrice
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Discount
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Promotion
    {
        public Discount discount { get; set; }
        public string message { get; set; }
        public string promotionCode { get; set; }
        public string promotionType { get; set; }
    }

    public class Seller
    {
        public string feedbackPercentage { get; set; }
        public string feedbackScore { get; set; }
        public string sellerAccountType { get; set; }
        public string username { get; set; }
    }

    public class BaseDeliveryCost
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class DeliveryDiscount
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class ShippingOption
    {
        public BaseDeliveryCost baseDeliveryCost { get; set; }
        public DeliveryDiscount deliveryDiscount { get; set; }
        public string maxEstimatedDeliveryDate { get; set; }
        public string minEstimatedDeliveryDate { get; set; }
        public string selected { get; set; }
        public string shippingCarrierCode { get; set; }
        public string shippingOptionId { get; set; }
        public string shippingServiceCode { get; set; }
    }

    public class LineItem
    {
        public BaseUnitPrice baseUnitPrice { get; set; }
        public Image image { get; set; }
        public string itemId { get; set; }
        public string lineItemId { get; set; }
        public NetPrice netPrice { get; set; }
        public List<Promotion> promotions { get; set; }
        public string quantity { get; set; }
        public Seller seller { get; set; }
        public List<ShippingOption> shippingOptions { get; set; }
        public string shortDescription { get; set; }
        public string title { get; set; }
    }

    public class Amount
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Adjustment
    {
        public Amount amount { get; set; }
        public string label { get; set; }
    }

    public class DeliveryCost
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class DeliveryDiscount2
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Fee
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class PriceDiscount
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class PriceSubtotal
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Tax
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class Total
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    public class PricingSummary
    {
        public Adjustment adjustment { get; set; }
        public DeliveryCost deliveryCost { get; set; }
        public DeliveryDiscount2 deliveryDiscount { get; set; }
        public Fee fee { get; set; }
        public PriceDiscount priceDiscount { get; set; }
        public PriceSubtotal priceSubtotal { get; set; }
        public Tax tax { get; set; }
        public Total total { get; set; }
    }

    public class PaymentInstrumentReference
    {
        public string lastFourDigitForCreditCard { get; set; }
    }

    public class LogoImage3
    {
        public string height { get; set; }
        public string imageUrl { get; set; }
        public string width { get; set; }
    }

    public class PaymentMethodBrand2
    {
        public LogoImage3 logoImage { get; set; }
        public string paymentMethodBrandType { get; set; }
    }

    public class ProvidedPaymentInstrument
    {
        public PaymentInstrumentReference paymentInstrumentReference { get; set; }
        public PaymentMethodBrand2 paymentMethodBrand { get; set; }
        public string paymentMethodType { get; set; }
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
