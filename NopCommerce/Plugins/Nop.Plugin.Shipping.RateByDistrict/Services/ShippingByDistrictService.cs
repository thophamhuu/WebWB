using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Shipping.RateByDistrict.Domain;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;

namespace Nop.Plugin.Shipping.RateByDistrict.Services
{
    public class ShippingByDistrictService : IShippingByDistrictService
    {
        #region Constants
        private const string SHIPPINGBYDISTRICT_ALL_KEY = "Nop.shippingbydistrict.all-{0}-{1}";
        private const string SHIPPINGBYDISTRICT_PATTERN_KEY = "Nop.shippingbydistrict.";
        #endregion

        #region Fields
        private readonly IRepository<ShippingByDistrictRecord> _sbdRepository;
        private readonly ICacheManager _cacheManager;

        #endregion
        #region Ctor
        public ShippingByDistrictService(IRepository<ShippingByDistrictRecord> sbdRepository,
            ICacheManager cacheManager)
        {
            this._cacheManager = cacheManager;
            this._sbdRepository = sbdRepository;
        }
        #endregion
        public virtual ShippingByDistrictRecord FindRecord(int shippingMethodId,
            int storeId, int warehouseId,
            int countryId, int stateProvinceId, string zip)
        {
            if (zip == null)
                zip = string.Empty;
            zip = zip.Trim();

            //filter by weight and shipping method
            var existingRates = GetAll()
                .Where(sbd => sbd.ShippingMethodId == shippingMethodId)
                .ToList();

            //filter by store
            var matchedByStore = new List<ShippingByDistrictRecord>();
            foreach (var sbd in existingRates)
                if (storeId == sbd.StoreId)
                    matchedByStore.Add(sbd);
            if (!matchedByStore.Any())
                foreach (var sbd in existingRates)
                    if (sbd.StoreId == 0)
                        matchedByStore.Add(sbd);

            //filter by country
            var matchedByCountry = new List<ShippingByDistrictRecord>();
            foreach (var sbd in matchedByStore)
                if (countryId == sbd.CountryId)
                    matchedByCountry.Add(sbd);
            if (!matchedByCountry.Any())
                foreach (var sbd in matchedByStore)
                    if (sbd.CountryId == 0)
                        matchedByCountry.Add(sbd);

            //filter by state/province
            var matchedByStateProvince = new List<ShippingByDistrictRecord>();
            foreach (var sbd in matchedByCountry)
                if (stateProvinceId == sbd.StateProvinceId)
                    matchedByStateProvince.Add(sbd);
            if (!matchedByStateProvince.Any())
                foreach (var sbd in matchedByCountry)
                    if (sbd.StateProvinceId == 0)
                        matchedByStateProvince.Add(sbd);


            //filter by zip
            var matchedByZip = new List<ShippingByDistrictRecord>();
            foreach (var sbd in matchedByStateProvince)
                if ((String.IsNullOrEmpty(zip) && String.IsNullOrEmpty(sbd.Zip)) ||
                    (zip.Equals(sbd.Zip, StringComparison.InvariantCultureIgnoreCase)))
                    matchedByZip.Add(sbd);

            if (!matchedByZip.Any())
                foreach (var taxRate in matchedByStateProvince)
                    if (String.IsNullOrWhiteSpace(taxRate.Zip))
                        matchedByZip.Add(taxRate);

            return matchedByZip.FirstOrDefault();
        }
        public virtual IPagedList<ShippingByDistrictRecord> GetAll(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string key = string.Format(SHIPPINGBYDISTRICT_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from sbd in _sbdRepository.Table
                            orderby sbd.StoreId, sbd.CountryId, sbd.StateProvinceId, sbd.Zip, sbd.ShippingMethodId
                            select sbd;

                var records = new PagedList<ShippingByDistrictRecord>(query, pageIndex, pageSize);
                return records;
            });
        }
        public virtual void InsertShippingByDistrictRecord(ShippingByDistrictRecord shippingByDistrictRecord)
        {
            if (shippingByDistrictRecord == null)
                throw new ArgumentNullException("shippingByDistrictRecord");

            _sbdRepository.Insert(shippingByDistrictRecord);

            _cacheManager.RemoveByPattern(SHIPPINGBYDISTRICT_PATTERN_KEY);
        }

        public virtual ShippingByDistrictRecord GetById(int shippingByDistrictRecordId)
        {
            if (shippingByDistrictRecordId == 0)
                return null;

            return _sbdRepository.GetById(shippingByDistrictRecordId);
        }

        public virtual void UpdateShippingByDistrictRecord(ShippingByDistrictRecord shippingByDistrictRecord)
        {
            if (shippingByDistrictRecord == null)
                throw new ArgumentNullException("shippingByDistrictRecord");

            _sbdRepository.Update(shippingByDistrictRecord);

            _cacheManager.RemoveByPattern(SHIPPINGBYDISTRICT_PATTERN_KEY);
        }
        public virtual void DeleteShippingByDistrictRecord(ShippingByDistrictRecord shippingByDistrictRecord)
        {
            if (shippingByDistrictRecord == null)
                throw new ArgumentNullException("shippingByDistrictRecord");

            _sbdRepository.Delete(shippingByDistrictRecord);

            _cacheManager.RemoveByPattern(SHIPPINGBYDISTRICT_PATTERN_KEY);
        }

        #region Utilities


        #endregion
    }
}
