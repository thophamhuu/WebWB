using System.IO;

namespace Nop.Plugin.Worldbuy.StateProvinceWB.Services
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public partial interface IStateProvinceWBImportManager
    {
        /// <summary>
        /// Import products from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        void ImportProvincesFromXlsx(Stream stream,int countryid);
    }
}
