using Nop.Core;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Services.Models.Responses
{
    public class SearchProductsResponse
    {
        public List<int> filterableSpecificationAttributeOptionIds { get; set; }
        public APIPagedList<Product> data { get; set; }
    }
}