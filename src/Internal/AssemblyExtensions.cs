using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace C3D.Core.DataAccess.Extensions
{
	internal static class AssemblyExtensions
	{
        public static IEnumerable<Type> GetAccessibleTypes(this Assembly assembly)
        {
            try
            {
                return assembly.DefinedTypes.Select(t => t.AsType());
            }
            catch (ReflectionTypeLoadException ex)
            {
                // The exception is thrown if some types cannot be loaded in partial trust.
                // For our purposes we just want to get the types that are loaded, which are
                // provided in the Types property of the exception.
                return ex.Types.Where(t => t != null);
            }
        }
    }
}
