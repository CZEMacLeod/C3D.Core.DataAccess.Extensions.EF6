using System;
using System.Reflection;

namespace C3D.Core.DataAccess.Extensions
{
	internal static class TypeExtensions
	{
		internal static bool InheritsGenericType(this Type type, Type genericType)
		{
			try
			{
				return type.BaseType != null &&
					type.BaseType.IsGenericType &&
					type.BaseType.GetGenericTypeDefinition() == genericType;
			}
			catch (Exception) { }
			return false;
		}

		internal static Type GetBaseGenericType(this Type configType, Type baseType, int level = 0)
		{
			if (configType == null) return null;
			if (configType.IsGenericType && configType.GetGenericTypeDefinition() == baseType) return configType;
			return GetBaseGenericType(configType.BaseType, baseType, level + 1);
		}

		internal static bool IsAnonymous(this Type type) =>
			Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false) &&
			type.IsGenericType &&
			type.Name.Contains("AnonymousType") &&
			(type.Name.StartsWith("<>") || type.Name.StartsWith("VB$")) &&
			type.Attributes.HasFlag(TypeAttributes.NotPublic);
	}
}
