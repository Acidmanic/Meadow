using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Meadow.Tools.Assistant.Compilation
{
    internal class Compiler
    {
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

        public static CSharpCompilation GenerateCSharpCode(string sourceCode)
        {
            var codeString = SourceText.From(sourceCode);


            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
            };

            Assembly.GetEntryAssembly()?.GetReferencedAssemblies().ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            return CSharpCompilation.Create("ClassListingAssembly",
                new[] {parsedSyntaxTree},
                references: references,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }

        private List<MetadataReference> CreateDefaultReferences()
        {
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                GetRuntimeSpecificReference()
            };


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