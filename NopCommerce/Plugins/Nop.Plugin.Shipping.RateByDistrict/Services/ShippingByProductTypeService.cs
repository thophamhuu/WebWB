using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Shipping.RateByDistrict.Domain;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;

namespace Nop.Plugin.Shipping.RateByDistrict.Services
{
    public class ShippingByProductTypeService : IShippingByProductTypeService
    {
        #region Constants
        private const string SHIPPINGByProductType_ALL_KEY = "Nop.shippingByProductType.all-{0}-{1}";
        private const string SHIPPINGByProductType_PATTERN_KEY = "Nop.shippingByProductType.";
        #endregion

        #region Fields
        private readonly IRepository<ShippingByProductTypeRecord> _sbdRepository;
        private readonly ICacheManager _cacheManager;

        #endregion
        #region Ctor
        public ShippingByProductTypeService(IRepository<ShippingByProductTypeRecord> sbdRepository,
            ICacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
            this._sbdRepository = sbdRepository;
        }
        #endregion
        public virtual ShippingByProductTypeRecord FindRecord(int storeId)
        {
            //filter by weight and shipping method
            var existingRates = GetAll()
                .Where(sbd => sbd.StoreId == storeId)
                .ToList();

            return existingRates.FirstOrDefault();
        }
        public virtual IPagedList<ShippingByProductTypeRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(SHIPPINGByProductType_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from sbd in _sbdRepository.Table
                            orderby sbd.StoreId
                            select sbd;

                var records = new PagedList<ShippingByProductTypeRecord>(query, pageIndex, pageSize);
                return records;
            });
        }
        public virtual void InsertShippingByProductTypeRecord(ShippingByProductTypeRecord shippingByProductTypeRecord)
        {
            if (shippingByProductTypeRecord == null)
                throw new ArgumentNullException("shippingByProductTypeRecord");

            _sbdRepository.Insert(shippingByProductTypeRecord);

            _cacheManager.RemoveByPattern(SHIPPINGByProductType_PATTERN_KEY);
        }

        public virtual ShippingByProductTypeRecord GetById(int shippingByProductTypeRecordId)
        {
            if (shippingByProductTypeRecordId == 0)
                return null;

            return _sbdRepository.GetById(shippingByProductTypeRecordId);
        }

        public virtual void UpdateShippingByProductTypeRecord(ShippingByProductTypeRecord shippingByProductTypeRecord)
        {
            if (shippingByProductTypeRecord == null)
                throw new ArgumentNullException("shippingByProductTypeRecord");

            _sbdRepository.Update(shippingByProductTypeRecord);

            _cacheManager.RemoveByPattern(SHIPPINGByProductType_PATTERN_KEY);
        }
        public virtual void DeleteShippingByProductTypeRecord(ShippingByProductTypeRecord shippingByProductTypeRecord)
        {
            if (shippingByProductTypeRecord == null)
                throw new ArgumentNullException("shippingByProductTypeRecord");

            _sbdRepository.Delete(shippingByProductTypeRecord);

            _cacheManager.RemoveByPattern(SHIPPINGByProductType_PATTERN_KEY);
        }

        #region Utilities


        #endregion
    }
}
