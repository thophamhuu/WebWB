﻿using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.CashAtWB.Models
{
    public class ConfigurationModel : BaseNopModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
    {
        public ConfigurationModel()
        {
            Locales = new List<ConfigurationLocalizedModel>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Plugins.Payment.CashAtWB.DescriptionText")]
        public string DescriptionText { get; set; }
        public bool DescriptionText_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.CashAtWB.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.CashAtWB.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payment.CashAtWB.ShippableProductRequired")]
        public bool ShippableProductRequired { get; set; }
        public bool ShippableProductRequired_OverrideForStore { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }

        #region Nested class

        public partial class ConfigurationLocalizedModel : ILocalizedModelLocal
        {
            public int LanguageId { get; set; }

            [AllowHtml]
            [NopResourceDisplayName("Plugins.Payment.CashAtWB.DescriptionText")]
            public string DescriptionText { get; set; }
        }

        #endregion

    }
}