using Nop.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Mvc;
using Autofac.Core;
using Nop.Data;
using Nop.Core.Data;
using Nop.Plugin.Worldbuy.StateProvinceWB.Services;
using Nop.Plugin.Worldbuy.StateProvinceWB.Data;
using Nop.Plugin.Worldbuy.StateProvinceWB.Domain;

namespace Nop.Plugin.Worldbuy.StateProvinceWB
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_state_province_wb";
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<StateProvinceWBService>().As<IStateProvinceWBService>().InstancePerLifetimeScope();
            builder.RegisterType<StateProvinceWBImportManager>().As<IStateProvinceWBImportManager>().InstancePerLifetimeScope();
            //data context
            this.RegisterPluginDataContext<StateProvinceWBObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<StateProvincePostalCode>>()
                .As<IRepository<StateProvincePostalCode>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }
    }
}
