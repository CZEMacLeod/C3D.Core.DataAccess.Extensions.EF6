using C3D.Core.DataAccess.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Data.Entity.ModelConfiguration
{
	public static class TypeConfigurationExtensions
	{
		internal static readonly Type GenericETCType = typeof(System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<>);
		internal static readonly Type GenericCTCType = typeof(System.Data.Entity.ModelConfiguration.ComplexTypeConfiguration<>);

		/// <summary>
		/// Returns whether a given type is an EntityTypeConfiguration
		/// </summary>
		/// <param name="type">Type to check</param>
		/// <returns>True if the type is dervied from System.Data.Entity.ModelConfiguration.EntityTypeConfiguration&lt;&gt;</returns>
		public static bool IsEntityTypeConfiguration(this Type type) =>
			type?.BaseType != null &&
			!type.ContainsGenericParameters &&
			type.InheritsGenericType(GenericETCType);

		/// <summary>
		/// Returns whether a given type is a ComplexTypeConfiguration
		/// </summary>
		/// <param name="type">Type to check</param>
		/// <returns>True if the type is dervied from System.Data.Entity.ModelConfiguration.ComplexTypeConfiguration&lt;&gt;</returns>
		public static bool IsComplexTypeConfiguration(this Type type) =>
			type?.BaseType != null &&
			!type.ContainsGenericParameters &&
			type.InheritsGenericType(GenericCTCType);

		/// <summary>
		/// Gets ITypeConfigurationInfo for all types in an assembly of <paramref name="types"/>
		/// Filters based on the namespace of the ModelItem and, if a *ConfigurationType, the *ConfigurationType
		/// Call the Add method on the return value to add these to the <paramref name="modelBuilder"/>
		/// </summary>
		/// <example>modelBuilder.GetAssemblyTypes(typeof(MyApp.Models.OM.User).Assembly, AssemblyTypeFilter.TypeConfigurations).Add()</example>
		/// <param name="modelBuilder">The DbModelBuilder to use</param>
		/// <param name="assembly">The assembly to scan</param>
		/// <param name="types">Filter to Entity Types (None) or Configuration Types</param>
		/// <param name="filter">Optional filter to restrict the returned types</param>
		/// <returns>An enumerable of ITypeConfigurationInfo based on the assembly, and filters</returns>
		public static IEnumerable<ITypeInfo> GetAssemblyTypes(this DbModelBuilder modelBuilder,
															  Assembly assembly,
															  AssemblyTypeFilter types,
															  Func<Type, bool> filter = null)
		{
			if (types == AssemblyTypeFilter.None) yield break;

			var q = assembly.GetAccessibleTypes().Where(t => !t.IsAbstract && !t.IsAnonymous());

			if (!types.HasFlag(AssemblyTypeFilter.EntityTypeConfiguration))
				q = q.Where(t => !t.IsEntityTypeConfiguration());
			if (!types.HasFlag(AssemblyTypeFilter.ComplexTypeConfiguration))
				q = q.Where(t => !t.IsComplexTypeConfiguration());
			if (!types.HasFlag(AssemblyTypeFilter.Model))
				q = q.Where(t => t.IsEntityTypeConfiguration() || t.IsComplexTypeConfiguration());
			if (filter != null)
				q = q.Where(filter);
			foreach (var t in q)
			{
				if (t.IsEntityTypeConfiguration())
				{
					yield return new EntityTypeConfigurationInfo(modelBuilder, t);
				}
				else if (t.IsComplexTypeConfiguration())
				{
					yield return new ComplexTypeConfigurationInfo(modelBuilder, t);
				}
				else
				{
					yield return new ModelTypeInfo(modelBuilder, t);
				}
			}
		}

		/// <summary>
		/// Gets ITypeConfigurationInfo for all types in an assembly of <paramref name="types"/>
		/// Filters based on the namespace of the ModelItem and, if a *ConfigurationType, the *ConfigurationType
		/// Call the Add method on the return value to add these to the <paramref name="modelBuilder"/>
		/// </summary>
		/// <example>modelBuilder.GetAssemblyTypes(typeof(MyApp.Models.OM.User).Assembly, AssemblyTypeFilter.TypeConfigurations, "MyApp.Models.OM").Add()</example>
		/// <param name="modelBuilder">The DbModelBuilder to use</param>
		/// <param name="assembly">The assembly to scan</param>
		/// <param name="types">Filter to Entity Types (None) or Configuration Types</param>
		/// <param name="nameSpace">The namespace to use to check the TypeConfiguration class or model class against</param>
		/// <returns>An enumerable of ITypeConfigurationInfo based on the assembly, and filters</returns>
		public static IEnumerable<ITypeInfo> GetAssemblyTypes(this DbModelBuilder modelBuilder,
															  Assembly assembly,
															  AssemblyTypeFilter types,
															  string nameSpace) =>
			string.IsNullOrEmpty(nameSpace) ?
				throw new ArgumentException($"'{nameof(nameSpace)}' cannot be null or empty.", nameof(nameSpace)) :
				modelBuilder.GetAssemblyTypes(assembly, types).Where(tci => tci.InNamespace(nameSpace));

		public static void Add(this IEnumerable<ITypeInfo> configurations)
		{
			foreach (var configuration in configurations)
			{
				configuration.Add();
			}
		}

		/// <summary>
		/// Registers an EntityTypeConfiguration&lt;&gt; or ComplexTypeConfiguration&lt;&gt; by type 
		/// rather than having to use the generic methods on ConfigurationRegistrar
		/// </summary>
		/// <param name="modelBuilder">The DbModelBuilder to use</param>
		/// <param name="type">A type that derives from EntityTypeConfiguration&lt;&gt; or ComplexTypeConfiguration&lt;&gt;</param>
		public static void RegisterTypeConfiguration(this DbModelBuilder modelBuilder, Type type)
		{
			if (type.IsEntityTypeConfiguration())
			{
				new EntityTypeConfigurationInfo(modelBuilder, type).Add();
			}
			else if (type.IsComplexTypeConfiguration())
			{
				new ComplexTypeConfigurationInfo(modelBuilder, type).Add();
			}
			else
			{
				throw new ArgumentException($"{type.Name} does not derive from EntityTypeConfiguration<> or ComplexTypeConfiguration<>", nameof(type));
			}
		}

		public static void AddFromAssembly(this DbModelBuilder modelBuilder,
									 Assembly assembly,
									 Func<Type, bool> filter = null) => 
			modelBuilder.GetAssemblyTypes(assembly, AssemblyTypeFilter.TypeConfigurations, filter).Add();

		public static IEnumerable<ITypeInfo> AddFromAssembly(this Configuration.ConfigurationRegistrar registrar,
															 Assembly assembly,
															 AssemblyTypeFilter types,
															 Func<Type, bool> filter = null)
		{
			if (types == AssemblyTypeFilter.None) yield break;
			if (types.HasFlag(AssemblyTypeFilter.Model)) throw new ArgumentOutOfRangeException(nameof(types));

			var q = assembly.GetAccessibleTypes().Where(t => !t.IsAbstract && !t.IsAnonymous());

			if (!types.HasFlag(AssemblyTypeFilter.EntityTypeConfiguration))
				q = q.Where(t => !t.IsEntityTypeConfiguration());
			if (!types.HasFlag(AssemblyTypeFilter.ComplexTypeConfiguration))
				q = q.Where(t => !t.IsComplexTypeConfiguration());
			if (!types.HasFlag(AssemblyTypeFilter.Model))
				q = q.Where(t => t.IsEntityTypeConfiguration() || t.IsComplexTypeConfiguration());
			if (filter != null)
				q = q.Where(filter);
			foreach (var t in q)
			{
				if (t.IsEntityTypeConfiguration())
				{
					yield return new EntityTypeConfigurationInfo(registrar, t);
				}
				else if (t.IsComplexTypeConfiguration())
				{
					yield return new ComplexTypeConfigurationInfo(registrar, t);
				}
			}
		}
	}
}
