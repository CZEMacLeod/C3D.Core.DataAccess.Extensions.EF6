using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;

namespace C3D.Core.DataAccess.Extensions
{
    internal class EntityTypeConfigurationInfo : TypeConfigurationInfo
    {
        protected override Type BaseGenericType => TypeConfigurationExtensions.GenericETCType;
        
        public EntityTypeConfigurationInfo(DbModelBuilder modelBuilder, Type type) : this(modelBuilder.Configurations, type) { }
        public EntityTypeConfigurationInfo(ConfigurationRegistrar registrar, Type type) : base(registrar, type) { }

        private static readonly MethodInfo addMethod = typeof(ConfigurationRegistrar).GetMethods().
            Single(m => m.Name == nameof(ConfigurationRegistrar.Add) &&
                   m.GetGenericArguments().Any(a => a.Name == "TEntityType"));
        internal override MethodInfo AddMethod() => addMethod.MakeGenericMethod(ModelType);
    }
}
