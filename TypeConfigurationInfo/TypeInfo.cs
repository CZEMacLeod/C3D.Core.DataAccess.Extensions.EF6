using System;
using System.Reflection;

namespace C3D.Core.DataAccess.Extensions
{
    internal abstract class TypeInfo : ITypeInfo
	{
		public static implicit operator Type (TypeInfo typeInfo) => typeInfo.ModelType;

		public abstract Type ModelType { get; }
		public abstract void Add();

		internal abstract MethodInfo AddMethod();

		public virtual bool InNamespace(string nameSpace) => ModelType.InNamespace(nameSpace);
	}
}
