using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;

namespace C3D.Core.DataAccess.Extensions
{
    internal class ComplexTypeConfigurationInfo : TypeConfigurationInfo
    {
        protected override Type BaseGenericType => TypeConfigurationExtensions.GenericCTCType;

        public ComplexTypeConfigurationInfo(DbModelBuilder modelBuilder, Type type) : this(modelBuilder.Configurations, type) { }
        public ComplexTypeConfigurationInfo(ConfigurationRegistrar registrar, Type type) : base(registrar, type) { }

        private static readonly MethodInfo addMethod = typeof(ConfigurationRegistrar).GetMethods().
            Single(m => m.Name == nameof(ConfigurationRegistrar.Add) &&
                   m.GetGenericArguments().Any(a => a.Name == "TComplexType"));

        internal override MethodInfo AddMethod() => addMethod.MakeGenericMethod(ModelType);
    }
}
