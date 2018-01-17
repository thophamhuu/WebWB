using Autofac;
using Autofac.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Data;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Domain;
using Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Services;
using Nop.Services.Tax;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Tax.Worldbuy.FixedOrByCountryStateZip.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
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
            //we cache presentation models between requests
            builder.RegisterType<WB_FixedOrByCountryStateZipTaxProvider>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));

            builder.RegisterType<WB_CountryStateZipService>().As<IWB_CountryStateZipService>().InstancePerLifetimeScope();
            builder.RegisterType<WB_TaxCategoryMappingService>().As<IWB_TaxCategoryMappingService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<WB_CountryStateZipObjectContext>(builder, "nop_object_context_tax_worldbuy_country_state_zip");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<WB_TaxRate>>()
                .As<IRepository<WB_TaxRate>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_tax_worldbuy_country_state_zip"))
                .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<WB_TaxCategoryMapping>>()
               .As<IRepository<WB_TaxCategoryMapping>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_tax_worldbuy_country_state_zip"))
               .InstancePerLifetimeScope();
            builder.RegisterType<WB_TaxService>()
                .As<ITaxService>()
                .InstancePerLifetimeScope();
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