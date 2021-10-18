using System;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace C3D.Core.DataAccess.Extensions
{
    internal abstract class TypeConfigurationInfo : TypeInfo
    {
        public static implicit operator Type(TypeConfigurationInfo typeInfo) => typeInfo.TypeConfigurationType;

        public Type TypeConfigurationType { get; }
        public Type GenericType { get; }
        public override Type ModelType { get; }

        private ConfigurationRegistrar registrar;

        protected abstract Type BaseGenericType { get; }

        protected TypeConfigurationInfo(ConfigurationRegistrar registrar, Type type)
        {
            this.registrar = registrar;
            TypeConfigurationType = type;
            GenericType = type.GetBaseGenericType(BaseGenericType);
            ModelType = GenericType.GetGenericArguments()[0];
        }

        private object CreateEntity() => Activator.CreateInstance(TypeConfigurationType);

        public override void Add() => AddMethod().Invoke(registrar, new[] { CreateEntity() });

        public override bool InNamespace(string nameSpace) =>
            base.InNamespace(nameSpace) || TypeConfigurationType.InNamespace(nameSpace);

        public override string ToString() => TypeConfigurationType.AssemblyQualifiedName;
    }
}
