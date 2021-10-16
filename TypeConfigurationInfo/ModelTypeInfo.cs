using System;
using System.Data.Entity;
using System.Reflection;

namespace C3D.Core.DataAccess.Extensions
{
	internal class ModelTypeInfo : TypeInfo
	{
		protected readonly DbModelBuilder ModelBuilder;

		public ModelTypeInfo(DbModelBuilder modelBuilder, Type modelType)
		{
			ModelType = modelType;
			ModelBuilder = modelBuilder;
		}

		public override Type ModelType { get; }

		private static readonly MethodInfo addMethod = typeof(DbModelBuilder).GetMethod(nameof(DbModelBuilder.Entity));
		internal override MethodInfo AddMethod() => addMethod.MakeGenericMethod(ModelType);

		//public override void Add() => modelBuilder.RegisterEntityType(modelType); 
		public override void Add() => AddMethod().Invoke(ModelBuilder, Type.EmptyTypes);
	}
}
