using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.Reflection;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;

namespace C3D.Core.DataAccess.Extensions
{
	internal abstract class TypeInfo : ITypeInfo
	{
		

		public abstract Type ModelType { get; }
		public abstract void Add();

		internal abstract MethodInfo AddMethod();

		public virtual bool InNamespace(string nameSpace) => ModelType.Namespace.StartsWith(nameSpace);
	}
}
