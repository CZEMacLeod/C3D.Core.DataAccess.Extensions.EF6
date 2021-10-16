using System;

namespace C3D.Core.DataAccess.Extensions
{
	public interface ITypeInfo
	{
		Type ModelType { get; }
		void Add();
		bool InNamespace(string nameSpace);
	}
}
