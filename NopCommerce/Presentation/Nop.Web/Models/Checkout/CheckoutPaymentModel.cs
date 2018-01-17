using System.Web.Routing;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Checkout
{
    public partial class CheckoutPaymentModel : BaseNopModel
    {
        public CheckoutPaymentModel()
        {
            this.CheckoutPaymentInfoModel = new CheckoutPaymentInfoModel();
            this.CheckoutPaymentMethodModel = new CheckoutPaymentMethodModel();
        }
        public CheckoutPaymentInfoModel CheckoutPaymentInfoModel { get; set; }
        public CheckoutPaymentMethodModel CheckoutPaymentMethodModel { get; set; }
    }
}