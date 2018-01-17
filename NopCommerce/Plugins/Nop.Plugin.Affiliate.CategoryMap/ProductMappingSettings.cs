using Nop.Core.Configuration;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Affiliate.CategoryMap
{
    public class ProductMappingSettings : ISettings
    {
        public decimal AdditionalCostPercent { get; set; }
        public string ShippingDescriptions { get; set; }
    }
}