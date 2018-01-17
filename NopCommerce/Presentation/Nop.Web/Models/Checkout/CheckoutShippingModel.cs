using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Checkout
{
    public partial class CheckoutShippingModel : BaseNopModel
    {
        public CheckoutShippingModel()
        {
            CheckoutShippingAddress = new CheckoutShippingAddressModel();
            CheckoutShippingMethod = new CheckoutShippingMethodModel();
        }
        public int AddressId { get; set; } = 0;
        public CheckoutShippingAddressModel CheckoutShippingAddress { get; set; }
        public CheckoutShippingMethodModel CheckoutShippingMethod { get; set; }
    }
}