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
        /// <param name="filter">Optional filter to restrict the returned types</param>
        /// <param name="types">Filter to Entity Types (None) or Configuration Types</param>
        /// <returns>An enumerable of ITypeConfigurationInfo based on the assembly, and filters</returns>
        public static IEnumerable<ITypeInfo> GetTypesFromAssembly(this DbModelBuilder modelBuilder,
                                                            Assembly assembly,
                                                            Func<Type, bool> filter = null,
                                                            AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations)
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

        public static IEnumerable<ITypeInfo> GetTypesFromAssemblies(this DbModelBuilder modelBuilder,
                                                            Func<Type, bool> filter = null,
                                                            AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations,
                                                            params Assembly[] assemblies) =>
            assemblies.SelectMany(assembly => modelBuilder.GetTypesFromAssembly(assembly, filter, types));

        public static IEnumerable<ITypeInfo> GetTypesFromAssemblyTypes(this DbModelBuilder modelBuilder,
                                                            Func<Type, bool> filter = null,
                                                            AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations,
                                                            params Type[] markerTypes) =>
            markerTypes.Select(t => t.Assembly).Distinct().SelectMany(assembly => modelBuilder.GetTypesFromAssembly(assembly, filter, types));

        public static IEnumerable<ITypeInfo> GetTypesFromAssemblyType(this DbModelBuilder modelBuilder,
                                                                 Type markerType,
                                                                 Func<Type, bool> filter = null,
                                                                AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations) =>
            modelBuilder.GetTypesFromAssembly(markerType.Assembly, filter, types);

        public static IEnumerable<ITypeInfo> GetTypesFromAssemblyType<TMarker>(this DbModelBuilder modelBuilder,
                                                    Func<Type, bool> filter = null,
                                                            AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations) =>
            modelBuilder.GetTypesFromAssemblyType(typeof(TMarker), filter, types);

        public static void AddFromAssembly(this DbModelBuilder modelBuilder,
                                     Assembly assembly,
                                     Func<Type, bool> filter = null,
                                     AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations)
        {
            modelBuilder.GetTypesFromAssembly(assembly, filter, types).Add();
        }

        /// <summary>
        /// Gets ITypeConfigurationInfo for all types in an assembly of <paramref name="types"/>
        /// Filters based on the namespace of the ModelItem and, if a *ConfigurationType, the *ConfigurationType
        /// Call the Add method on the return value to add these to the <paramref name="modelBuilder"/>
        /// </summary>
        /// <example>modelBuilder.GetAssemblyTypes(typeof(MyApp.Models.OM.User).Assembly, AssemblyTypeFilter.TypeConfigurations, "MyApp.Models.OM").Add()</example>
        /// <param name="modelBuilder">The DbModelBuilder to use</param>
        /// <param name="assembly">The assembly to scan</param>
        /// <param name="nameSpace">The namespace to use to check the TypeConfiguration class or model class against</param>
        /// <param name="filter">Optional filter to restrict the returned types</param>
        /// <param name="types">Filter to Entity Types (None) or Configuration Types</param>
        /// <returns>An enumerable of ITypeConfigurationInfo based on the assembly, and filters</returns>
        public static void AddFromAssembly(this DbModelBuilder modelBuilder,
                                           Assembly assembly,
                                           string nameSpace,
                                           Func<Type, bool> filter = null,
                                           AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations)
        {
            if (string.IsNullOrEmpty(nameSpace)) throw new ArgumentException($"'{nameof(nameSpace)}' cannot be null or empty.", nameof(nameSpace));
            modelBuilder.GetTypesFromAssembly(assembly, filter, types).Where(tci => tci.InNamespace(nameSpace)).Add();
        }

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

        public static IEnumerable<ITypeInfo> GetTypesFromAssembly(this Configuration.ConfigurationRegistrar registrar,
                                                             Assembly assembly,
                                                             Func<Type, bool> filter = null,
                                                             AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations)
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


        public static IEnumerable<ITypeInfo> GetTypesFromAssemblies(this Configuration.ConfigurationRegistrar registrar,
                                                            Func<Type, bool> filter = null,
                                                            AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations,
                                                            params Assembly[] assemblies) =>
            assemblies.SelectMany(assembly => registrar.GetTypesFromAssembly(assembly, filter, types));

        public static IEnumerable<ITypeInfo> GetTypesFromAssemblyTypes(this Configuration.ConfigurationRegistrar registrar,
                                                            Func<Type, bool> filter = null,
                                                            AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations,
                                                            params Type[] markerTypes) =>
            markerTypes.Select(t => t.Assembly).Distinct().SelectMany(assembly => registrar.GetTypesFromAssembly(assembly, filter, types));

        public static IEnumerable<ITypeInfo> GetTypesFromAssemblyType(this Configuration.ConfigurationRegistrar registrar,
                                                                 Type markerType,
                                                                 Func<Type, bool> filter = null,
                                                                AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations) =>
            registrar.GetTypesFromAssembly(markerType.Assembly, filter, types);

        public static IEnumerable<ITypeInfo> GetTypesFromAssemblyType<TMarker>(this Configuration.ConfigurationRegistrar registrar,
                                                    Func<Type, bool> filter = null,
                                                            AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations) =>
            registrar.GetTypesFromAssemblyType(typeof(TMarker), filter, types);

        public static void AddFromAssembly(this Configuration.ConfigurationRegistrar registrar,
                                     Assembly assembly,
                                     Func<Type, bool> filter = null,
                                     AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations) => 
            registrar.GetTypesFromAssembly(assembly, filter, types).Add();

        public static void AddFromAssembly(this Configuration.ConfigurationRegistrar registrar,
                                           Assembly assembly,
                                           string nameSpace,
                                           Func<Type, bool> filter = null,
                                           AssemblyTypeFilter types = AssemblyTypeFilter.TypeConfigurations)
        {
            if (string.IsNullOrEmpty(nameSpace)) throw new ArgumentException($"'{nameof(nameSpace)}' cannot be null or empty.", nameof(nameSpace));
            registrar.GetTypesFromAssembly(assembly, filter, types).Where(tci => tci.InNamespace(nameSpace)).Add();
        }
    }
}
