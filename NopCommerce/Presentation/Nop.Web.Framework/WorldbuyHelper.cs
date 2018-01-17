using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace Nop.Web.Framework
{
    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class WorldbuyHelper
    {
        public static string TaxFormatter(string value, bool isAbsolute)
        {
            string str = value;
            if (isAbsolute)
            {
                var _workContext = EngineContext.Current.Resolve<IWorkContext>();
                var _currencyService = EngineContext.Current.Resolve<ICurrencyService>();
                var _priceFormater = EngineContext.Current.Resolve<IPriceFormatter>();

                decimal tax = 0M;

                if (!string.IsNullOrEmpty(value))
                {
                    string taxStr = value;
                    Regex rg = new Regex(@"[\d]+(.[\d][\d]*)?");
                    string currencyCode = rg.Replace(value, "");
                    if (!String.IsNullOrEmpty(currencyCode))
                    {
                        taxStr = value.Replace(currencyCode, "");
                    }
                    Decimal.TryParse(taxStr, out tax);
                }
                tax = _currencyService.ConvertFromPrimaryStoreCurrency(tax, _workContext.WorkingCurrency);
                str = tax.ToString(_workContext.WorkingCurrency.CustomFormatting);


            }
            return str;
        }
    }
}
