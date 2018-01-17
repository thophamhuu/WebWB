using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.ExportImport.Help;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using OfficeOpenXml;
using Nop.Plugin.Worldbuy.StateProvinceWB.Domain;

namespace Nop.Plugin.Worldbuy.StateProvinceWB.Services
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class StateProvinceWBImportManager : IStateProvinceWBImportManager
    {
        #region Fields

        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStateProvinceWBService _stateProvinceWBService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public StateProvinceWBImportManager(
            IStateProvinceService stateProvinceService,
            IStateProvinceWBService stateProvinceWBService,
            IWorkContext workContext,
            ILocalizationService localizationService)
        {
            this._stateProvinceService = stateProvinceService;
            this._stateProvinceWBService = stateProvinceWBService;
            this._workContext = workContext;
            this._localizationService = localizationService;
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

        public void ImportProvincesFromXlsx(Stream stream, int countryId)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                //the columns
                var properties = GetPropertiesByExcelCells<StateProvince>(worksheet);

                var manager = new PropertyManager<StateProvince>(properties);

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



                    //var seName = string.Empty;
                    string name = "";
                    string postalCode = "";
                    string abbreviation = "";
                    foreach (var property in manager.GetProperties)
                    {
                        switch (property.PropertyName)
                        {
                            case "Name":
                                name = property.StringValue;
                                break;
                            case "PostalCode":
                                postalCode = property.StringValue;
                                break;
                            case "Abbreviation":
                                abbreviation = property.StringValue;
                                break;
                        }
                    }

                    var stateProvince = _stateProvinceService.GetStateProvinceByAbbreviation(abbreviation) ?? new StateProvince
                    {
                        Published = true,
                        CountryId = countryId,
                        Name = name,
                        Abbreviation = abbreviation,
                        Id = 0
                    };

                    var isNew = stateProvince.Id == 0;
                    if (isNew)
                    {
                        _stateProvinceService.InsertStateProvince(stateProvince);
                    }
                    else
                    {
                        _stateProvinceService.UpdateStateProvince(stateProvince);

                    }
                    if (stateProvince.Id > 0)
                    {
                        var postalCodes = postalCode.Split(',').ToList();
                        if (postalCodes != null)
                        {
                            foreach (var postal in postalCodes)
                            {
                                var stateProvinceWB = _stateProvinceWBService.GetByPostalCodeAndProvinceID(postal,stateProvince.Id) ?? new StateProvincePostalCode
                                {
                                    Id = 0,
                                    StateProvinceID = stateProvince.Id,
                                    PostalCode = postal
                                };
                                if (stateProvinceWB.Id == 0)
                                {
                                    _stateProvinceWBService.Insert(stateProvinceWB);
                                }
                            }

                        }
                    }
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
