using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Shipping.RateByDistrict.Domain;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Shipping.RateByDistrict.Models;

namespace Nop.Plugin.Shipping.RateByDistrict.Services
{
    public class ShippingByCategoryService : IShippingByCategoryService
    {
        #region Constants
        private const string ShippingByCategory_ALL_KEY = "Nop.ShippingByCategory.all-{0}-{1}-{2}";
        private const string ShippingByCategory_PATTERN_KEY = "Nop.ShippingByCategory.";
        #endregion

        #region Fields
        private readonly IRepository<ShippingByCategoryRecord> _sbcRepository;
        private readonly IShippingByProductTypeService _shippingByProductTypeService;
        private readonly ICacheManager _cacheManager;

        #endregion
        #region Ctor
        public ShippingByCategoryService(IRepository<ShippingByCategoryRecord> sbdRepository,
            IShippingByProductTypeService shippingByProductTypeService,
            ICacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
            this._shippingByProductTypeService = shippingByProductTypeService;
            this._sbcRepository = sbdRepository;
        }
        #endregion
        public virtual ShippingByCategoryRecord FindRecord(int storeId, int productTypeId, int categoryId)
        {
            //filter by weight and shipping method
            var existingRates = GetAll(productTypeId)
                .Where(sbc => sbc.ProductTypeId == productTypeId && sbc.CategoryId == categoryId)
                .ToList();

            //filter by store
            var matchedByStore = new List<ShippingByCategoryRecord>();
            foreach (var sbd in existingRates)
                if (storeId == sbd.StoreId)
                    matchedByStore.Add(sbd);
            if (!matchedByStore.Any())
                foreach (var sbd in existingRates)
                    if (sbd.StoreId == 0)
                        matchedByStore.Add(sbd);



            return matchedByStore.FirstOrDefault();
        }
        public virtual IPagedList<ShippingByCategoryRecord> GetAll(int productTypeId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(ShippingByCategory_ALL_KEY, productTypeId, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from sbd in _sbcRepository.Table
                            orderby sbd.StoreId, sbd.ProductTypeId
                            select sbd;

                if (productTypeId > 0)
                {
                    query = query.Where(x => x.ProductTypeId == productTypeId).OrderBy(x=>x.StoreId).ThenBy(x=>x.ProductTypeId);
                }
                var records = new PagedList<ShippingByCategoryRecord>(query, pageIndex, pageSize);
                return records;
            });
        }
        public virtual void InsertShippingByCategoryRecord(ShippingByCategoryRecord ShippingByCategoryRecord)
        {
            if (ShippingByCategoryRecord == null)
                throw new ArgumentNullException("ShippingByCategoryRecord");

            _sbcRepository.Insert(ShippingByCategoryRecord);

            _cacheManager.RemoveByPattern(ShippingByCategory_PATTERN_KEY);
        }

        public virtual ShippingByCategoryRecord GetById(int ShippingByCategoryRecordId)
        {
            if (ShippingByCategoryRecordId == 0)
                return null;

            return _sbcRepository.GetById(ShippingByCategoryRecordId);
        }

        public virtual void UpdateShippingByCategoryRecord(ShippingByCategoryRecord ShippingByCategoryRecord)
        {
            if (ShippingByCategoryRecord == null)
                throw new ArgumentNullException("ShippingByCategoryRecord");

            _sbcRepository.Update(ShippingByCategoryRecord);

            _cacheManager.RemoveByPattern(ShippingByCategory_PATTERN_KEY);
        }
        public virtual void DeleteShippingByCategoryRecord(ShippingByCategoryRecord ShippingByCategoryRecord)
        {
            if (ShippingByCategoryRecord == null)
                throw new ArgumentNullException("ShippingByCategoryRecord");

            _sbcRepository.Delete(ShippingByCategoryRecord);

            _cacheManager.RemoveByPattern(ShippingByCategory_PATTERN_KEY);
        }

        public ShippingByProductTypeModel GetProductTypeByCategoryId(int Id)
        {
            var category = _sbcRepository.TableNoTracking.Where(x => x.CategoryId == Id).FirstOrDefault();
            if (category != null)
            {
                var productType = _shippingByProductTypeService.GetById(category.ProductTypeId);
                if (productType != null)
                    return new ShippingByProductTypeModel
                    {
                        AdditionalFixedCost = productType.AdditionalFixedCost,
                        Id = productType.Id,
                        ProductTypeName = productType.ProductTypeName,
                    };
            }
            return null;
        }

        public ShippingByCategoryModel GetCategoryByCategoryId(int Id)
        {
            var category = _sbcRepository.TableNoTracking.Where(x => x.CategoryId == Id).FirstOrDefault();
            if (category != null)
            {
                var categoryModel = new ShippingByCategoryModel
                {
                    AdditionalFixedCost = category.AdditionalFixedCost,
                    CategoryId = category.CategoryId,
                    Id = category.Id,
                    ProductTypeId = category.ProductTypeId,
                    StoreId = category.StoreId,
                };
                return categoryModel;
            }
            return null;
        }

        #region Utilities


        #endregion
    }
}
