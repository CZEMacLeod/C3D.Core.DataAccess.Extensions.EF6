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
		public Type EntityType { get; }

		public ComplexTypeConfigurationInfo(DbModelBuilder modelBuilder, Type entityType) :
			this(modelBuilder.Configurations, entityType)
		{ }

		public ComplexTypeConfigurationInfo(ConfigurationRegistrar registrar, Type entityType) : base(registrar)

		{
			EntityType = entityType;
			ModelType = entityType.GetBaseGenericType(TypeConfigurationExtensions.GenericCTCType);
		}

		public override Type ModelType { get; }

		private static readonly MethodInfo addMethod = typeof(ConfigurationRegistrar).GetMethods().
			Single(m => m.Name == nameof(ConfigurationRegistrar.Add) &&
				   m.GetGenericArguments().Any(a => a.Name == "TComplexType"));
		internal override MethodInfo AddMethod() => addMethod.MakeGenericMethod(ModelType);

		//public override void Add() => modelBuilder.RegisterEntityType(modelType); 
		public override void Add() => AddMethod().Invoke(registrar, Type.EmptyTypes);

		public override bool InNamespace(string nameSpace) =>
			base.InNamespace(nameSpace) || EntityType.Namespace.StartsWith(nameSpace);
	}
}
