using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Mantle.Collections;
using Microsoft.Extensions.DependencyModel;

namespace Mantle.Reflection
{
    /// <summary>
    /// http://stackoverflow.com/questions/37895278/how-to-load-assemblies-located-in-a-folder-in-net-core-console-app
    /// </summary>
    public class AssemblyLoader : AssemblyLoadContext
    {
        // Not exactly sure about this
        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (DependencyContext.Default.CompileLibraries.Any(x=>x.Name.Equals(assemblyName.Name, StringComparison.OrdinalIgnoreCase)))
            {
                var assembly = Assembly.Load(assemblyName);
                return assembly;
            }
            return null; // TODO: throw error instead?

            //var compileLibraries = DependencyContext.Default.CompileLibraries
            //    .Where(d => d.Name.Equals(assemblyName.Name, StringComparison.OrdinalIgnoreCase))
            //    .ToList();

            //if (compileLibraries.IsNullOrEmpty())
            //{
            //    return null; // TODO: throw error instead?
            //}

            //var assembly = Assembly.Load(new AssemblyName(compileLibraries.First().Name));
            //return assembly;
        }
    }
}