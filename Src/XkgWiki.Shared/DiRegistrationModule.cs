using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;

namespace XkgWiki.Shared
{
    public abstract class DiRegistrationModuleBase : Module
    {
        private readonly Func<IRegistrationBuilder<object, object, object>, IRegistrationBuilder<object, object, object>> _lifetimeScopeConfigurator;

        protected DiRegistrationModuleBase(
            Func<IRegistrationBuilder<object, object, object>, IRegistrationBuilder<object, object, object>> lifetimeScopeConfigurator)
        {
            _lifetimeScopeConfigurator = lifetimeScopeConfigurator;
        }

        protected sealed override void Load(ContainerBuilder builder)
        {
            foreach (var registrationBuilder in RegisterTypesWithDefaultLifetimeScope(builder))
            {
                _lifetimeScopeConfigurator(registrationBuilder).PropertiesAutowired();
            }
        }

        protected virtual IEnumerable<IRegistrationBuilder<object, object, object>> RegisterTypesWithDefaultLifetimeScope(ContainerBuilder builder)
        {
            return Enumerable.Empty<IRegistrationBuilder<object, object, object>>();
        }
    }
}