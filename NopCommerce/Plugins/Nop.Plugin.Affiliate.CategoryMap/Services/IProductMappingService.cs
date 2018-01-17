using Nop.Plugin.Affiliate.CategoryMap.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.CategoryMap.Services
{
    public interface IProductMappingService
    {
        ProductMapping GetProductBySourceId(string productSourceId, int source);
        void InsertProduct(ProductMapping product);
        ProductMapping GetProductMappingByProductId(int productId);
        List<ProductMapping> GetAllProductMappingBySource(int sourceId);
        void UpdateProductMapping(ProductMapping product);
    }
}
