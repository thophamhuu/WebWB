using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Domain;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Services
{
    /// <summary>
    /// Tax Category Mappipng service interface
    /// </summary>
    public partial interface IWB_TaxCategoryMappingService
    {
        /// <summary>
        /// Deletes a Tax Category Mappipng
        /// </summary>
        /// <param name="taxCategoryMapping">Tax Category Mappipng</param>
        void DeleteTaxCategoryMapping(WB_TaxCategoryMapping taxCategoryMapping);

        /// <summary>
        /// Gets all Tax Category Mappipngs
        /// </summary>
        /// <returns>Tax Category Mappipngs</returns>
        IPagedList<WB_TaxCategoryMapping> GetAllTaxCategoryMappings(int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets a Tax Category Mappipng
        /// </summary>
        /// <param name="taxCategoryMappingId">Tax Category Mappipng identifier</param>
        /// <returns>Tax Category Mappipng</returns>
        WB_TaxCategoryMapping GetTaxCategoryMappingById(int taxCategoryMappingId);

        /// <summary>
        /// Inserts a Tax Category Mappipng
        /// </summary>
        /// <param name="taxCategoryMapping">Tax Category Mappipng</param>
        void InsertTaxCategoryMapping(WB_TaxCategoryMapping taxCategoryMapping);

        /// <summary>
        /// Updates the Tax Category Mappipng
        /// </summary>
        /// <param name="taxCategoryMapping">Tax Category Mappipng</param>
        void UpdateTaxCategoryMapping(WB_TaxCategoryMapping taxCategoryMapping);
        int GetTaxCategoryId(Product product);
    }
}