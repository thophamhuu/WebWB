using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.Affiliate.CategoryMap.Services
{
    public class ProductMappingService : IProductMappingService
    {
        private readonly IRepository<ProductMapping> _productMappingRepository;
        public ProductMappingService(IRepository<ProductMapping> productMappingRepository)
        {
            this._productMappingRepository = productMappingRepository;
        }
        public ProductMapping GetProductBySourceId(string productSourceId, int source)
        {
            if (String.IsNullOrEmpty(productSourceId))
                return null;

            productSourceId = productSourceId.Trim();

            var query = from p in _productMappingRepository.Table
                        orderby p.Id
                        where
                        p.ProductSourceId == productSourceId &&
                        p.SourceId == source
                        select p;
            var product = query.FirstOrDefault();
            return product;
        }

        public void InsertProduct(ProductMapping product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            //insert
            _productMappingRepository.Insert(product);
        }

        public ProductMapping GetProductMappingByProductId(int productId)
        {
            if (productId <= 0)
                return null;

            var query = from p in _productMappingRepository.Table
                        where p.ProductId == productId
                        select p;

            var product = query.FirstOrDefault();
            return product;
        }

        public List<ProductMapping> GetAllProductMappingBySource(int sourceId)
        {
            var query = from cr in _productMappingRepository.Table
                        where cr.SourceId == sourceId
                        select cr;
            var data = query.ToList();
            return data;
        }

        public void UpdateProductMapping(ProductMapping product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            _productMappingRepository.Update(product);
        }
    }
}
