//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection.PortableExecutable;
//using Microsoft.AspNetCore.Mvc.ApplicationParts;
//using Microsoft.AspNetCore.Mvc.Razor.Compilation;
//using Microsoft.CodeAnalysis;
//using Microsoft.Extensions.DependencyModel;

//namespace Mantle.Web.Mvc.Razor.Compilation
//{
//    // NOTE: This is a workaround for an issue in .NET Core 2. See: https://github.com/dotnet/core-setup/issues/2981#issuecomment-322572374
//    // TODO: Remove this once fix is deployed
//    public class ReferencesMetadataReferenceFeatureProvider : IApplicationFeatureProvider<MetadataReferenceFeature>
//    {
//        public void PopulateFeature(IEnumerable<ApplicationPart> parts, MetadataReferenceFeature feature)
//        {
//            var libraryPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
//            foreach (var assemblyPart in parts.OfType<AssemblyPart>())
//            {
//                var dependencyContext = DependencyContext.Load(assemblyPart.Assembly);
//                if (dependencyContext != null)
//                {
//                    foreach (var library in dependencyContext.CompileLibraries)
//                    {
//                        if (string.Equals("reference", library.Type, StringComparison.OrdinalIgnoreCase))
//                        {
//                            foreach (var libraryAssembly in library.Assemblies)
//                            {
//                                libraryPaths.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, libraryAssembly));
//                            }
//                        }
//                        else
//                        {
//                            foreach (var path in library.ResolveReferencePaths())
//                            {
//                                libraryPaths.Add(path);
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    libraryPaths.Add(assemblyPart.Assembly.Location);
//                }
//            }

//            foreach (var path in libraryPaths)
//            {
//                feature.MetadataReferences.Add(CreateMetadataReference(path));
//            }
//        }

//        private static MetadataReference CreateMetadataReference(string path)
//        {
//            using (var stream = File.OpenRead(path))
//            {
//                var moduleMetadata = ModuleMetadata.CreateFromStream(stream, PEStreamOptions.PrefetchMetadata);
//                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);

//                return assemblyMetadata.GetReference(filePath: path);
//            }
//        }
//    }
//}