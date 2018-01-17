using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Plugin.Affiliate.Ebay.Domain;
using Nop.Plugin.Affiliate.Ebay.Models;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.Ebay.Services
{
    public partial class AffiliateEbayService : IAffiliateEbayService
    {
        #region Constants

        /// <summary>
        /// Key pattern to clear cache
        /// </summary>
        private const string PRODUCTS_PATTERN_KEY = "Nop.product.";

        #endregion

        #region Fields

        private readonly IRepository<CategoryEbay> _categoryEbayRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductMapping> _productMappingRepository;
        private readonly IRepository<CategoryMapping> _categoryMapRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<SpecificationAttribute> _specificationAttributeRepository;

        #endregion

        #region Ctor

        public AffiliateEbayService(IRepository<CategoryEbay> categoryEbayRepository, ICacheManager cacheManager, IEventPublisher eventPublisher, IRepository<Category> categoryRepository,
            IRepository<Product> productRepository, IRepository<ProductMapping> productMappingRepository, IRepository<CategoryMapping> categoryMapRepository,
            IRepository<SpecificationAttribute> specificationAttributeRepository)
        {
            this._categoryEbayRepository = categoryEbayRepository;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._productRepository = productRepository;
            this._productMappingRepository = productMappingRepository;
            this._categoryMapRepository = categoryMapRepository;
            this._categoryRepository = categoryRepository;
            this._specificationAttributeRepository = specificationAttributeRepository;
        }

        #endregion

        #region Methods

        ///// <summary>
        ///// Gets all categories
        ///// </summary>
        ///// <param name="pageIndex">Page index</param>
        ///// <param name="pageSize">Page size</param>
        ///// <returns>Categories</returns>

        public IPagedList<CategoryEbayModel> GetAllCategories(int pageIndex = 0, int pageSize = int.MaxValue)
        {           
            var query = (from cA in _categoryEbayRepository.TableNoTracking
                         select new CategoryEbayModel
                         {
                             EbayId = cA.EbayId,
                             Name = cA.Name,
                             Id = cA.Id,
                             Published = cA.Published,
                             Deleted = cA.Deleted,
                             ParentCategoryId = cA.ParentCategoryId,
                             Level = cA.Level
                         }).ToList();
            query.ForEach(x =>
            {
                var map = _categoryMapRepository.TableNoTracking.FirstOrDefault(m => m.CategorySourceId == x.Id && m.SourceId == (int)Source.Ebay);
                if (map != null)
                {
                    var cate = _categoryRepository.GetById(map.CategoryId);
                    if (cate != null)
                    {
                        x.CategoryID = cate.Id;
                        x.CategoryMapID = map.Id;
                        x.CategoryName = cate.Name;
                    }
                }
            });

            var result = TreeView(query);

            return new PagedList<CategoryEbayModel>(query, pageIndex, pageSize);
        }

        public virtual void DeleteCategoryEbay(CategoryEbay categoryEbay)
        {
            if (categoryEbay == null)
                throw new ArgumentNullException("categoryEbay");

            _categoryEbayRepository.Delete(categoryEbay);
        }

        public virtual void InsertCategoryEbay(CategoryEbay categoryEbay)
        {
            if (categoryEbay == null)
                throw new ArgumentNullException("categoryEbay");

            _categoryEbayRepository.Insert(categoryEbay);
        }

        public virtual void UpdateCategoryEbay(CategoryEbay categoryEbay)
        {
            if (categoryEbay == null)
                throw new ArgumentNullException("categoryEbay");

            _categoryEbayRepository.Update(categoryEbay);
        }

        public virtual CategoryEbay GetByEbayId(int ebayId)
        {
            if (ebayId == 0)
                return null;

            var query = from gp in _categoryEbayRepository.Table
                        where gp.EbayId == ebayId
                        orderby gp.Id
                        select gp;
            var record = query.FirstOrDefault();
            return record;
        }

        /// <summary>
        /// Gets a product by SourceID
        /// </summary>
        /// <param name="productSourceId">SourceID</param>
        /// <param name="source">Source</param>
        /// <returns>Product</returns>
        public virtual Product GetProductBySourceId(string productSourceId, int source)
        {
            if (String.IsNullOrEmpty(productSourceId))
                return null;

            productSourceId = productSourceId.Trim();

            var query = from p in _productMappingRepository.Table
                        where p.ProductSourceId == productSourceId &&
                        p.SourceId == source
                        select p;
            var productMapping = query.FirstOrDefault();

            var product = new Product();
            if (productMapping != null)
                product = _productRepository.GetById(productMapping.ProductId);

            return product;
        }

        public virtual IList<CategoryEbay> GetAll()
        {
            var query = from gp in _categoryEbayRepository.Table
                        orderby gp.Id
                        select gp;
            var records = query.ToList();
            return records;
        }

        public CategoryEbay Get(int id)
        {
            return _categoryEbayRepository.Table.FirstOrDefault(m => m.Id == id);
        }

        public CategoryMapping MapCategory(int id, int categoryId)
        {
            var categoryMap = _categoryMapRepository.Table.FirstOrDefault(x => x.CategoryId == categoryId && x.CategorySourceId == id) ?? new CategoryMapping
            {
                CategoryId = categoryId,
                CategorySourceId = id,
                SourceId = (int)Source.Ebay
            };
            if (categoryMap.Id == 0)
            {
                _categoryMapRepository.Insert(categoryMap);
            }
            else
            {
                categoryMap.CategoryId = categoryId;
                _categoryMapRepository.Update(categoryMap);
            }
            return categoryMap;
        }

        public void RemoveMapCategory(int id)
        {
            var categoryMap = _categoryMapRepository.GetById(id);
            if (categoryMap != null)
            {
                _categoryMapRepository.Delete(categoryMap);
            }
            else
            {
                throw new ArgumentNullException("categoryMap");
            }
        }

        private IEnumerable<CategoryEbayModel> TreeView(IList<CategoryEbayModel> source, int parentId = 0, string parentName = "")
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var result = new List<CategoryEbayModel>();
            if (parentName != null && parentName != "")
                parentName = parentName + " >> ";
            foreach (var cat in source.Where(c => c.ParentCategoryId == parentId || parentId == 0).ToList())
            {
                cat.Name = parentName + cat.Name;
                if (!result.Contains(cat))
                {
                    result.Add(cat);
                    result.AddRange(TreeView(source, cat.Id, cat.Name));
                }
            }

            return result;
        }

        public IPagedList<Product> GetAllProduct(int pageIndex = 0, int pageSize = int.MaxValue, IList<int> categoryIds = null)
        {
            IQueryable <Product> query = null;

            var pIds = new List<int>();
            pIds = _productMappingRepository.TableNoTracking.Where(x => x.SourceId == (int)Source.Ebay).Select(x => x.ProductId).ToList();          

            if (pIds != null)
            {
                query = _productRepository.Table;
                query = query.Where(p => !p.Deleted);
                //query = query.OrderBy(c => c.Id).ThenBy(c => c.DisplayOrder).ThenBy(c => c.Name);
                query = from p in query
                        where pIds.Contains(p.Id)
                        select p;

                //category filtering
                if (categoryIds != null && categoryIds.Any())
                {
                    query = from p in query
                            from pc in p.ProductCategories.Where(pc => categoryIds.Contains(pc.CategoryId))
                            select p;
                }               
            }

            return new PagedList<Product>(query.OrderBy(c => c.Id), pageIndex, pageSize);
        }

        public virtual SpecificationAttribute GetSpecificationAttributeByName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return null;
           
            var query = from cr in _specificationAttributeRepository.Table
                        orderby cr.Id
                        where cr.Name == name
                        select cr;
            var attribute = query.FirstOrDefault();
            return attribute;
        }
        #endregion
    }
}
