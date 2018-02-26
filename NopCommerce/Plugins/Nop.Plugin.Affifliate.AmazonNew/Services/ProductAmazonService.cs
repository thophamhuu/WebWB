using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Plugin.Affiliate.Amazon.Models;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Directory;

namespace Nop.Plugin.Affiliate.Amazon.Services
{
    public partial class ProductAmazonService : IProductAmazonService
    {
        #region Fields
        private readonly IProductService _productService;


        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<ProductMapping> _productMappingRepository;

        private readonly IRepository<ProductPicture> _productPictureRepository;
        private readonly IRepository<Picture> _pictureRepository;
        private readonly IPictureService _pictureService;
        private readonly ICategoryService _categoryService;

        #endregion
        #region ctr
        public ProductAmazonService(IProductService productService, IPictureService pictureService, IRepository<Picture> pictureRepository, IRepository<ProductPicture> productPictureRepository, ICategoryService categoryService, IRepository<Product> productRepository, IRepository<ProductCategory> productCategoryRepository, IRepository<ProductMapping> productMappingRepository)
        {
            this._productService = productService;
            this._pictureService = pictureService;
            this._pictureRepository = pictureRepository;
            this._productPictureRepository = productPictureRepository;
            this._categoryService = categoryService;
            this._productRepository = productRepository;
            this._productCategoryRepository = productCategoryRepository;
            this._productMappingRepository = productMappingRepository;
        }

        public IPagedList<ProductAmazonModel> GetAllByCategoryId(ProductParameter model, int pageIndex = 1, int pageSize = 25)
        {
            var _amazonProvider = EngineContext.Current.Resolve<IAmazonProvider>();
            var result = _amazonProvider.GetAllProduct(model);

            int count = result.Count();
            var ids = result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToArray();
            //var data = result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            if (ids != null)
            {
                //var ids = data.Select(x => x.ProductId).ToArray();
                var maps = _productMappingRepository.TableNoTracking.Where(m => ids.Contains(m.ProductId)).ToList();
                var data = new List<ProductAmazonModel>();
                var _currencyService = EngineContext.Current.Resolve<ICurrencyService>();
                ids.ToList().ForEach(x =>
                {
                    var product = _productService.GetProductById(x);
                    var map = maps.FirstOrDefault(m => m.ProductId == x);
                    if (map != null && product != null)
                    {
                        var item = new ProductAmazonModel
                        {
                            ASIN = map.ProductSourceId,
                            DetailUrl = map.ProductSourceLink,
                            Id = map.Id,
                            Name = product.Name,
                            Price = _currencyService.ConvertFromPrimaryStoreCurrency(product.Price, _currencyService.GetCurrencyByCode("USD")).ToString(),
                            PriceSource = map.Price.ToString(),
                            ProductId = product.Id
                        };
                        var productPicture = _productPictureRepository.TableNoTracking.OrderBy(pp => pp.DisplayOrder).FirstOrDefault(pp => pp.ProductId == x);
                        if (productPicture != null)
                        {
                            item.Image = _pictureService.GetPictureUrl(productPicture.PictureId, 0);
                        }
                        data.Add(item);
                    }

                });
                return new PagedList<ProductAmazonModel>(data, pageIndex, pageSize, count);
            }
            return null;

        }

        public IEnumerable<ProductMapping> GetAllProductMappingByCategoryId(int categoryId, bool? isPublished = null, int page = 1, int size = 100)
        {
            var _amazonProvider = EngineContext.Current.Resolve<IAmazonProvider>();
            var result = _amazonProvider.GetAllProductMapping(new ProductParameter { CategoryID = categoryId, IsPublished = isPublished });
            if (result != null)
            {
                result = result.Skip((page - 1) * size).Take(size).ToList();
                var query = from p in _productMappingRepository.Table
                            orderby p.Id
                            where result.Contains(p.ProductId)
                            select p;

                return query.ToList();
            }

            return null;
        }
        #endregion

    }
}
