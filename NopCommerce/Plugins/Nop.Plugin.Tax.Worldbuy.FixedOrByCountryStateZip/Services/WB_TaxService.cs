using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Catalog;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Services
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial class WB_TaxService : TaxService
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly TaxSettings _taxSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IGeoLookupService _geoLookupService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILogger _logger;
        private readonly CustomerSettings _customerSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly AddressSettings _addressSettings;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="addressService">Address service</param>
        /// <param name="workContext">Work context</param>
        /// <param name="storeContext">Store context</param>
        /// <param name="taxSettings">Tax settings</param>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="geoLookupService">GEO lookup service</param>
        /// <param name="countryService">Country service</param>
        /// <param name="stateProvinceService">State province service</param>
        /// <param name="logger">Logger service</param>
        /// <param name="customerSettings">Customer settings</param>
        /// <param name="shippingSettings">Shipping settings</param>
        /// <param name="addressSettings">Address settings</param>
        public WB_TaxService(IAddressService addressService,
            IWorkContext workContext,
            IStoreContext storeContext,
            TaxSettings taxSettings,
            IPluginFinder pluginFinder,
            IGeoLookupService geoLookupService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ILogger logger,
            CustomerSettings customerSettings,
            ShippingSettings shippingSettings,
            AddressSettings addressSettings) : base(addressService,
            workContext,
            storeContext,
            taxSettings,
            pluginFinder,
            geoLookupService,
            countryService,
            stateProvinceService,
            logger,
            customerSettings,
            shippingSettings,
            addressSettings)
        {
            this._addressService = addressService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._taxSettings = taxSettings;
            this._pluginFinder = pluginFinder;
            this._geoLookupService = geoLookupService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._logger = logger;
            this._customerSettings = customerSettings;
            this._shippingSettings = shippingSettings;
            this._addressSettings = addressSettings;
        }
        protected virtual decimal CalculatePrice(decimal price, decimal percent, bool increase, bool isAbsolute)
        {
            if (percent == decimal.Zero)
                return price;

            decimal result;
            if (isAbsolute)
            {
                if (increase)
                {
                    result = price + percent;
                }
                else
                {
                    result = price - percent;
                }
            }
            else
            {
                if (increase)
                {
                    result = price * (1 + percent / 100);
                }
                else
                {
                    result = price - (price) / (100 + percent) * percent;
                }
            }
            return result;
        }
        protected virtual CalculateTaxRequest CreateCalculateTaxRequest(Product product,
           int taxCategoryId, Customer customer, decimal price, out int taxCategoryIdOut)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var calculateTaxRequest = new CalculateTaxRequest
            {
                Customer = customer,
                Product = product,
                Price = price,
                TaxCategoryId = taxCategoryId > 0 ? taxCategoryId : (product != null ? product.TaxCategoryId : 0)
            };
            taxCategoryIdOut = calculateTaxRequest.TaxCategoryId;
            var basedOn = _taxSettings.TaxBasedOn;

            //new EU VAT rules starting January 1st 2015
            //find more info at http://ec.europa.eu/taxation_customs/taxation/vat/how_vat_works/telecom/index_en.htm#new_rules
            var overridenBasedOn = _taxSettings.EuVatEnabled                                            //EU VAT enabled?
                && product != null && product.IsTelecommunicationsOrBroadcastingOrElectronicServices    //telecommunications, broadcasting and electronic services?
                && DateTime.UtcNow > new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc)                //January 1st 2015 passed?
                && IsEuConsumer(customer);                                                              //Europe Union consumer?
            if (overridenBasedOn)
            {
                //We must charge VAT in the EU country where the customer belongs (not where the business is based)
                basedOn = TaxBasedOn.BillingAddress;
            }

            //tax is based on pickup point address
            if (!overridenBasedOn && _taxSettings.TaxBasedOnPickupPointAddress && _shippingSettings.AllowPickUpInStore)
            {
                var pickupPoint = customer.GetAttribute<PickupPoint>(SystemCustomerAttributeNames.SelectedPickupPoint, _storeContext.CurrentStore.Id);
                if (pickupPoint != null)
                {
                    var country = _countryService.GetCountryByTwoLetterIsoCode(pickupPoint.CountryCode);
                    var state = _stateProvinceService.GetStateProvinceByAbbreviation(pickupPoint.StateAbbreviation);

                    calculateTaxRequest.Address = new Address
                    {
                        Address1 = pickupPoint.Address,
                        City = pickupPoint.City,
                        Country = country,
                        CountryId = country.Return(c => c.Id, 0),
                        StateProvince = state,
                        StateProvinceId = state.Return(sp => sp.Id, 0),
                        ZipPostalCode = pickupPoint.ZipPostalCode,
                        CreatedOnUtc = DateTime.UtcNow
                    };

                    return calculateTaxRequest;
                }
            }

            if (basedOn == TaxBasedOn.BillingAddress && customer.BillingAddress == null ||
                basedOn == TaxBasedOn.ShippingAddress && customer.ShippingAddress == null)
            {
                basedOn = TaxBasedOn.DefaultAddress;
            }

            switch (basedOn)
            {
                case TaxBasedOn.BillingAddress:
                    calculateTaxRequest.Address = customer.BillingAddress;
                    break;
                case TaxBasedOn.ShippingAddress:
                    calculateTaxRequest.Address = customer.ShippingAddress;
                    break;
                case TaxBasedOn.DefaultAddress:
                default:
                    calculateTaxRequest.Address = _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId);
                    break;
            }

            return calculateTaxRequest;
        }
        protected virtual void GetTaxRate(Product product, int taxCategoryId,
            Customer customer, decimal price, out decimal taxRate, out bool isTaxable, out int taxCategoryIdOut)
        {
            taxRate = decimal.Zero;
            isTaxable = true;
            taxCategoryIdOut = taxCategoryId;
            //active tax provider
            var activeTaxProvider = LoadActiveTaxProvider(customer);
            if (activeTaxProvider == null)
                return;
            //tax request
            var calculateTaxRequest = this.CreateCalculateTaxRequest(product, taxCategoryId, customer, price, out taxCategoryIdOut);

            //tax exempt
            if (IsTaxExempt(product, calculateTaxRequest.Customer))
            {
                isTaxable = false;
            }
            //make EU VAT exempt validation (the European Union Value Added Tax)
            if (isTaxable &&
                _taxSettings.EuVatEnabled &&
                IsVatExempt(calculateTaxRequest.Address, calculateTaxRequest.Customer))
            {
                //VAT is not chargeable
                isTaxable = false;
            }

            //get tax rate
            var calculateTaxResult = activeTaxProvider.GetTaxRate(calculateTaxRequest);
            if (calculateTaxResult.Success)
            {
                //ensure that tax is equal or greater than zero
                if (calculateTaxResult.TaxRate < decimal.Zero)
                    calculateTaxResult.TaxRate = decimal.Zero;

                taxRate = calculateTaxResult.TaxRate;
            }
            else
                if (_taxSettings.LogErrors)
            {
                foreach (var error in calculateTaxResult.Errors)
                {
                    _logger.Error(string.Format("{0} - {1}", activeTaxProvider.PluginDescriptor.FriendlyName, error), null, customer);
                }
            }
        }
        public override decimal GetProductPrice(Product product, int taxCategoryId, decimal price, bool includingTax, Customer customer, bool priceIncludesTax, out decimal taxRate)
        {
            if (price == decimal.Zero)
            {
                taxRate = decimal.Zero;
                return taxRate;
            }


            bool isTaxable;
            var _taxCategoryMappingService = EngineContext.Current.Resolve<IWB_TaxCategoryMappingService>();
            taxCategoryId = _taxCategoryMappingService.GetTaxCategoryId(product);

            this.GetTaxRate(product, taxCategoryId, customer, price, out taxRate, out isTaxable, out taxCategoryId);
            var _settingService = EngineContext.Current.Resolve<ISettingService>();
            bool isAbsolute = _settingService.GetSettingByKey<bool>(string.Format("Tax.Worldbuy.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}.IsAbsolute", taxCategoryId));


            if (priceIncludesTax)
            {
                //"price" already includes tax
                if (includingTax)
                {
                    //we should calculate price WITH tax
                    if (!isTaxable)
                    {
                        //but our request is not taxable
                        //hence we should calculate price WITHOUT tax
                        price = CalculatePrice(price, taxRate, false, isAbsolute);
                    }
                }
                else
                {
                    //we should calculate price WITHOUT tax
                    price = CalculatePrice(price, taxRate, false, isAbsolute);
                }
            }
            else
            {
                //"price" doesn't include tax
                if (includingTax)
                {
                    //we should calculate price WITH tax
                    //do it only when price is taxable
                    if (isTaxable)
                    {
                        price = CalculatePrice(price, taxRate, true, isAbsolute);
                    }
                }
            }


            if (!isTaxable)
            {
                //we return 0% tax rate in case a request is not taxable
                taxRate = decimal.Zero;
            }

            //allowed to support negative price adjustments
            //if (price < decimal.Zero)
            //    price = decimal.Zero;

            return price;
        }


        #endregion
    }
}
