using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Api.Models.Responses
{
    public class UpdateShoppingCartItemResponse
    {
        public IList<string> warnings { get; set; }
        public ShoppingCartItem ShoppingCartItem { get; set; }
    }
}