using Nop.Core;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Domain;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Services
{
    /// <summary>
    /// Tax rate service interface
    /// </summary>
    public partial interface IWB_CountryStateZipService
    {
        /// <summary>
        /// Deletes a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        void DeleteTaxRate(WB_TaxRate taxRate);

        /// <summary>
        /// Gets all tax rates
        /// </summary>
        /// <returns>Tax rates</returns>
        IPagedList<WB_TaxRate> GetAllTaxRates(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="taxRateId">Tax rate identifier</param>
        /// <returns>Tax rate</returns>
        WB_TaxRate GetTaxRateById(int taxRateId);

        /// <summary>
        /// Inserts a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        void InsertTaxRate(WB_TaxRate taxRate);

        /// <summary>
        /// Updates the tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        void UpdateTaxRate(WB_TaxRate taxRate);
    }
}