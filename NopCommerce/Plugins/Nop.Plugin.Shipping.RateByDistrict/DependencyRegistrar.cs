using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Shipping.RateByDistrict.Data;
using Nop.Plugin.Shipping.RateByDistrict.Domain;
using Nop.Plugin.Shipping.RateByDistrict.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Shipping.RateByDistrict
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
            builder.RegisterType<ShippingByDistrictService>().As<IShippingByDistrictService>().InstancePerLifetimeScope();
            builder.RegisterType<ShippingByProductTypeService>().As<IShippingByProductTypeService>().InstancePerLifetimeScope();
            builder.RegisterType<ShippingByCategoryService>().As<IShippingByCategoryService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<ShippingByDistrictObjectContext>(builder, "nop_object_context_shipping_district_zip");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<ShippingByDistrictRecord>>()
                .As<IRepository<ShippingByDistrictRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_shipping_district_zip"))
                .InstancePerLifetimeScope();

            //override required repository with our custom context
            builder.RegisterType<EfRepository<ShippingByProductTypeRecord>>()
                .As<IRepository<ShippingByProductTypeRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_shipping_district_zip"))
                .InstancePerLifetimeScope();

            //override required repository with our custom context
            builder.RegisterType<EfRepository<ShippingByCategoryRecord>>()
                .As<IRepository<ShippingByCategoryRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_shipping_district_zip"))
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
