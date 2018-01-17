using Autofac;
using Autofac.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Affiliate.CategoryMap.Data;
using Nop.Plugin.Affiliate.CategoryMap.Domain;
using Nop.Plugin.Affiliate.CategoryMap.Services;
using Nop.Services.Catalog;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Affiliate.CategoryMap
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //data context
            this.RegisterPluginDataContext<CategoryMappingObjectContext>(builder, "nop_object_context_affiliate_categorymapping");

            builder.RegisterType<CategoryMappingService>().As<ICategoryMappingService>().InstancePerLifetimeScope().WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                .InstancePerLifetimeScope(); ;
            builder.RegisterType<ProductMappingService>().As<IProductMappingService>().InstancePerLifetimeScope();

            //override required repository with our custom context
            builder.RegisterType<EfRepository<CategoryMapping>>()
                .As<IRepository<CategoryMapping>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_affiliate_categorymapping"))
                .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<ProductMapping>>()
               .As<IRepository<ProductMapping>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_affiliate_categorymapping"))
               .InstancePerLifetimeScope();


            //builder.RegisterType<WB_PriceCalculationService>()
            //    .As<IPriceCalculationService>()
            //    .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1; }
        }
    }
}
