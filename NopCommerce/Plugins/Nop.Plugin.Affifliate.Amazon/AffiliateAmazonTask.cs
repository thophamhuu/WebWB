using Nop.Services.Tasks;
using System;
using Nop.Core.Domain.Tasks;
using Nop.Core.Data;
using Nop.Plugin.Affiliate.Amazon.Services;
using Nop.Plugin.Affiliate.Amazon.Models;
using Nop.Services.Stores;
using Nop.Core;
using Nop.Services.Common;
using Nop.Core.Domain.Customers;
using System.Threading;
using Nop.Services.Configuration;
using Nop.Core.Plugins;
using System.Web;
using Nop.Core.Infrastructure;
using Nop.Plugin.Affiliate.CategoryMap.Domain;

namespace Nop.Plugin.Affiliate.Amazon
{
    public partial class AffiliateAmazonTask : ITask
    {
        #region Fields

        private readonly IRepository<ScheduleTask> _taskRepository;
        private readonly IAmazonService _amazonService;
        private readonly IProductAmazonService _productAmazonService;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public AffiliateAmazonTask(IRepository<ScheduleTask> taskRepository,
            IAmazonService amazonService,
            IProductAmazonService productAmazonService,
            IStoreService storeService,
            ISettingService settingService,
            IWorkContext workContext,
            IPluginFinder pluginFinder)
        {
            this._taskRepository = taskRepository;
            this._amazonService = amazonService;
            this._productAmazonService = productAmazonService;
            this._storeService = storeService;
            this._settingService = settingService;
            this._workContext = workContext;
        }

        #endregion
        #region Method
        public void Execute()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);

            var settings = _settingService.LoadSetting<AffiliateAmazonSettings>(storeScope);
            var hourInDay = DateTime.Now.Hour;

            SyncProperties syncProperties = SyncProperties.Price | SyncProperties.Variations;
            var context = HttpContext.Current;
            var _productMappingRepo = EngineContext.Current.Resolve<IRepository<ProductMapping>>();
            _amazonService.UpdateProducts(_productMappingRepo, storeScope, 0, syncProperties);
        }
        #endregion

        #region Utilities
        public virtual int GetActiveStoreScopeConfiguration(IStoreService storeService, IWorkContext workContext)
        {
            //ensure that we have 2 (or more) stores
            if (storeService.GetAllStores().Count < 2)
                return 0;
            var storeId = workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
            var store = storeService.GetStoreById(storeId);
            return store != null ? store.Id : 0;
        }
        #endregion
    }
}
