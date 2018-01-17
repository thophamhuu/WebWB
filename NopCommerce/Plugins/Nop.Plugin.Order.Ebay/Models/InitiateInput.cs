using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Order.Ebay.Models
{
    public class InitiateInput
    {
        public string contactEmail { get; set; }
        public string contactFirstName { get; set; }
        public string contactLastName { get; set; }
        public CreditCard creditCard { get; set; }
        public List<LineItemInput> lineItemInputs { get; set; }
        public ShippingAddress shippingAddress { get; set; }
    }

    public class BillingAddress
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string county { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string postalCode { get; set; }
        public string stateOrProvince { get; set; }
    }

    public class CreditCard
    {
        public string accountHolderName { get; set; }
        public BillingAddress billingAddress { get; set; }
        public string brand { get; set; }
        public string cardNumber { get; set; }
        public string cvvNumber { get; set; }
        public int expireMonth { get; set; }
        public int expireYear { get; set; }
    }

    public class LineItemInput
    {
        public string itemId { get; set; }
        public int quantity { get; set; }
    }

    public class ShippingAddress
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string county { get; set; }
        public string phoneNumber { get; set; }
        public string postalCode { get; set; }
        public string recipient { get; set; }
        public string stateOrProvince { get; set; }
    }
}
