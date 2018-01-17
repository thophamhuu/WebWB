using System.IO;

namespace Nop.Plugin.Affiliate.Amazon.Services
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public partial interface IAffiliateAmazonImportManager
    {
        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        void ImportCategoryAmazonFromXlsx(Stream stream);
    }
}
