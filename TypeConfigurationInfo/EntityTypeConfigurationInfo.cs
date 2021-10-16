﻿using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;

namespace C3D.Core.DataAccess.Extensions
{
	internal class EntityTypeConfigurationInfo : TypeConfigurationInfo
	{
		public Type EntityType { get; }

		public EntityTypeConfigurationInfo(DbModelBuilder modelBuilder, Type entityType) :
			this(modelBuilder.Configurations, entityType)
		{ }

		public EntityTypeConfigurationInfo(ConfigurationRegistrar registrar, Type entityType) : base(registrar)
		{
			EntityType = entityType;
			ModelType = entityType.GetBaseGenericType(TypeConfigurationExtensions.GenericETCType);
		}

		public override Type ModelType { get; }

		private static readonly MethodInfo addMethod = typeof(ConfigurationRegistrar).GetMethods().
			Single(m=>m.Name == nameof(ConfigurationRegistrar.Add) && 
				   m.GetGenericArguments().Any(a=>a.Name== "TEntityType"));
		internal override MethodInfo AddMethod() => addMethod.MakeGenericMethod(ModelType);

		//public override void Add() => modelBuilder.RegisterEntityType(modelType); 
		public override void Add() => AddMethod().Invoke(registrar, Type.EmptyTypes);

		public override bool InNamespace(string nameSpace) =>
			base.InNamespace(nameSpace) || EntityType.Namespace.StartsWith(nameSpace);
	}
}