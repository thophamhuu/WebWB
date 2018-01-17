using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Infrastructure.Cache;
using Nop.Services.Events;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Services
{
    /// <summary>
    /// Tax rate service
    /// </summary>
    public partial class WB_CountryStateZipService : IWB_CountryStateZipService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<WB_TaxRate> _taxRateRepository;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="taxRateRepository">Tax rate repository</param>
        public WB_CountryStateZipService(IEventPublisher eventPublisher,
            ICacheManager cacheManager,
            IRepository<WB_TaxRate> taxRateRepository)
        {
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
            this._taxRateRepository = taxRateRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public virtual void DeleteTaxRate(WB_TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException("taxRate");

            _taxRateRepository.Delete(taxRate);

            //event notification
            _eventPublisher.EntityDeleted(taxRate);
        }

        /// <summary>
        /// Gets all tax rates
        /// </summary>
        /// <returns>Tax rates</returns>
        public virtual IPagedList<WB_TaxRate> GetAllTaxRates(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(WB_ModelCacheEventConsumer.TAXRATE_ALL_KEY, pageIndex, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var query = from tr in _taxRateRepository.Table
                            orderby tr.StoreId, tr.CountryId, tr.StateProvinceId, tr.Zip, tr.TaxCategoryId
                            select tr;
                var records = new PagedList<WB_TaxRate>(query, pageIndex, pageSize);
                return records;
            });
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="taxRateId">Tax rate identifier</param>
        /// <returns>Tax rate</returns>
        public virtual WB_TaxRate GetTaxRateById(int taxRateId)
        {
            if (taxRateId == 0)
                return null;

           return _taxRateRepository.GetById(taxRateId);
        }

        /// <summary>
        /// Inserts a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public virtual void InsertTaxRate(WB_TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException("taxRate");

            _taxRateRepository.Insert(taxRate);

            //event notification
            _eventPublisher.EntityInserted(taxRate);
        }

        /// <summary>
        /// Updates the tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public virtual void UpdateTaxRate(WB_TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException("taxRate");

            _taxRateRepository.Update(taxRate);

            //event notification
            _eventPublisher.EntityUpdated(taxRate);
        }

        #endregion
    }
}