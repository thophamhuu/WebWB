using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Services.ExportImport.Help;
using OfficeOpenXml;
using Nop.Plugin.Affiliate.Amazon.Domain;

namespace Nop.Plugin.Affiliate.Amazon.Services
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class AffiliateAmazonImportManager : IAffiliateAmazonImportManager
    {
        #region Fields

        private readonly ICategoryAmazonService _categoryAmazonSevice;

        #endregion

        #region Ctor

        public AffiliateAmazonImportManager(
            ICategoryAmazonService categoryAmazonSevice
         )
        {
            this._categoryAmazonSevice = categoryAmazonSevice;
        }

        #endregion

        #region Utilities

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string ConvertColumnToString(object columnValue)
        {
            if (columnValue == null)
                return null;

            return Convert.ToString(columnValue);
        }

        protected virtual string GetMimeTypeFromFilePath(string filePath)
        {
            var mimeType = MimeMapping.GetMimeMapping(filePath);

            //little hack here because MimeMapping does not contain all mappings (e.g. PNG)
            if (mimeType == MimeTypes.ApplicationOctetStream)
                mimeType = MimeTypes.ImageJpeg;

            return mimeType;
        }



        #endregion

        #region Methods

        public void ImportCategoryAmazonFromXlsx(Stream stream)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = GetPropertiesByExcelCells<CategoryAmazon>(worksheet);

                var manager = new PropertyManager<CategoryAmazon>(properties);

                var iRow = 2;
                var setSeName = properties.Any(p => p.PropertyName == "SeName");

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadFromXlsx(worksheet, iRow);

                    var categoryAmazon = _categoryAmazonSevice.GetByBrowseNodeID(manager.GetProperty("BrowseNodeID").StringValue);

                    var isNew = categoryAmazon == null;

                    categoryAmazon = categoryAmazon ?? new CategoryAmazon
                    {
                        BrowseNodeID = "",
                        IsCategoryRoot = false,
                        Name = "",
                        ParentBrowseNodeID = "",
                        SearchIndex = ""
                    };

                    var seName = string.Empty;

                    foreach (var property in manager.GetProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "Name":
                                categoryAmazon.Name = property.StringValue;
                                break;
                            case "BrowseNodeID":
                                categoryAmazon.BrowseNodeID = property.StringValue;
                                break;
                            case "ParentBrowseNodeID":
                                categoryAmazon.ParentBrowseNodeID = property.StringValue ;
                                break;
                            case "SearchIndex":
                                categoryAmazon.SearchIndex = property.StringValue;
                                break;
                            case "IsCategoryRoot":
                                categoryAmazon.IsCategoryRoot = property.StringValue == "1" ? true : false;
                                break;
                        }
                    }
                    if (isNew)
                        _categoryAmazonSevice.Insert(categoryAmazon);
                    else
                        _categoryAmazonSevice.Update(categoryAmazon);

                    //search engine name

                    iRow++;
                }

            }
        }
        #endregion


        protected virtual IList<PropertyByName<T>> GetPropertiesByExcelCells<T>(ExcelWorksheet worksheet)
        {
            var properties = new List<PropertyByName<T>>();
            var poz = 1;
            while (true)
            {
                try
                {
                    var cell = worksheet.Cells[1, poz];

                    if (cell == null || cell.Value == null || string.IsNullOrEmpty(cell.Value.ToString()))
                        break;

                    poz += 1;
                    properties.Add(new PropertyByName<T>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }

            return properties;
        }
    }
}
