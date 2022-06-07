using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Meadow.Tools.Assistant.Compilation.ProjectReferences;
using Meadow.Tools.Assistant.DotnetProject;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;

namespace Meadow.Tools.Assistant.Compilation
{
    public class DirectoryCompiler
    {
        private readonly Nuget.Nuget _nuget;
        private readonly DirectoryInfo _tempDir;

        public DirectoryCompiler()
        {
            var executionPath = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent?.FullName;

            var nugetCachePath = Path.Join(executionPath, "nugetCache");

            var tempPath = Path.Join(executionPath, "temp");

            _tempDir = new DirectoryInfo(tempPath);

            if (_tempDir.Exists)
            {
                _tempDir.Delete(true);
            }

            _tempDir.Create();

            _nuget = new Nuget.Nuget(nugetCachePath);
        }

        public DirectoryCompiler WithLocalNuGetDirectory(params string[] directories)
        {
            if (directories != null)
            {
                foreach (var directory in directories)
                {
                    _nuget.AddLocalDirectoryPackageSource(directory);
                }
            }

            return this;
        }


        public CompiledDirectoryResult Compile(string directory)
        {
            var result = new CompiledDirectoryResult();

            result.ProjectsOnPath = DotnetProjectInfo.FindProjects(directory);

            var mergedProject = new MergedProject(result.ProjectsOnPath);

            result.AllIncludedProjects = mergedProject.InvolvedProjects;

            result.AllCSharpFiles = mergedProject.SourceFiles;

            result.Assembly = CompileTogether(mergedProject.SourceFiles, mergedProject.PackageReferences);

            return result;
        }


        private List<FileInfo> IncludeNuGets(List<MetadataReference> references, List<PackageReference> nuGets)
        {
            var runtimes = new List<FileInfo>();

            if (nuGets != null)
            {
                foreach (var nuget in nuGets)
                {
                    var runtimeSet = _nuget
                        .GetCompilingReferences(nuget.PackageName, nuget.PackageVersion);

                    runtimeSet.ForEach(rt => references.Add(MetadataReference.CreateFromFile(rt.FullName)));

                    runtimes.AddRange(runtimeSet);
                }
            }

            return runtimes;
        }

        private Assembly CompileTogether(List<FileInfo> files, List<PackageReference> nugets = null)
        {
            var codes = Read(files);

            var parsedCodes = Parse(codes);

            var references = CreateDefaultReferences();

            var runtimes = IncludeNuGets(references, nugets);

            var cSharpCompilation = CSharpCompilation.Create("ClassListingAssembly",
                parsedCodes,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));

            using (var peStream = new MemoryStream())
            {
                var compilationResult = cSharpCompilation.Emit(peStream);

                if (!compilationResult.Success)
                {
                    Console.WriteLine("Compilation done with error.");

                    var failures = compilationResult.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return null;
                }

                Console.WriteLine("Compilation done without any error.");

                peStream.Seek(0, SeekOrigin.Begin);

                var assBytes = peStream.ToArray();

                var writeAssembly = WriteAssembly(assBytes, "ClassListingAssembly");

                runtimes.ForEach(file => file.CopyTo(Path.Join(_tempDir.FullName, file.Name), true));

                var assembly = Assembly.LoadFrom(writeAssembly);

                return assembly;
            }
        }

        private string WriteAssembly(byte[] bytes, string packageName)
        {
            var filePath = Path.Join(_tempDir.FullName, packageName + ".dll");

            File.WriteAllBytes(filePath, bytes);

            return filePath;
        }

        private List<string> Read(List<FileInfo> files)
        {
            var result = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    var content = File.ReadAllText(file.FullName);

                    result.Add(content);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return result;
        }

        private List<SyntaxTree> Parse(List<string> codes)
        {
            var parsedCodes = new List<SyntaxTree>();

            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);

            foreach (var code in codes)
            {
                var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(code, options);

                parsedCodes.Add(parsedSyntaxTree);
            }

            return parsedCodes;
        }

        private List<MetadataReference> CreateDefaultReferences()
        {
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
                GetRuntimeSpecificReference()
            };

            var file = new FileInfo(typeof(object).Assembly.Location).Directory;

            var others = file.EnumerateFiles("*.dll");

            foreach (var other in others)
            {
                try
                {
                    references.Add(MetadataReference.CreateFromFile(other.FullName));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            return references;
        }

        private static string GetAssemblyLocation<T>()
        {
            var typeOfT = typeof(T);

            return typeOfT.Assembly.Location;
        }

        private static PortableExecutableReference GetMetadataReference<T>()
        {
            var assemblyLocation = GetAssemblyLocation<T>();

            return MetadataReference.CreateFromFile(assemblyLocation);
        }

        // This function was needed
        private static PortableExecutableReference GetRuntimeSpecificReference()
        {
            var assemblyLocation = GetAssemblyLocation<object>();
            var runtimeDirectory = Path.GetDirectoryName(assemblyLocation);
            var libraryPath = Path.Join(runtimeDirectory, @"netstandard.dll");

            return MetadataReference.CreateFromFile(libraryPath);
        }
    }
}