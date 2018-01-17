using Nop.Core;
using Nop.Core.Domain.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nop.Plugin.Affiliate.CategoryMap.Domain
{
    public class ProductMapping : BaseEntity, ILocalizedEntity
    {
        public int ProductId { get; set; }
        public string ProductSourceId { get; set; }
        public string ProductSourceLink { get; set; }
        public int SourceId { get; set; }
        public decimal Price { get; set; }
        public string VariationsXML { get; set; }
        public IList<Variation> Variations
        {
            get
            {
                var variations = new List<Variation>();
                if (!String.IsNullOrEmpty(VariationsXML))
                {
                    XElement xmlVariations = XElement.Parse(VariationsXML);
                    if (xmlVariations != null)
                    {
                        var lstVariation = xmlVariations.Elements("Variation");
                        if (lstVariation != null)
                        {
                            foreach (XElement variation in lstVariation)
                            {
                                var var = new Variation
                                {
                                    Sku = variation.Attribute("Sku").ToString(),
                                    Price = Decimal.Parse(variation.Value)
                                };
                                variations.Add(var);
                            }
                        }
                    }
                }
                return new List<Variation>();
            }
        }
    }
    public class Variation
    {
        public string Sku { get; set; }
        public decimal Price { get; set; }
    }
}
