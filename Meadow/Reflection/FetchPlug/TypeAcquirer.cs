using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Meadow.Reflection.FetchPlug
{
    public class TypeAcquirer
    {
        // private IEnumerable<Type> SafeGetModuleTypes(Module module)
        // {
        //     try
        //     {
        //         return module.GetTypes();
        //     }
        //     catch (Exception _)
        //     {
        //         // ignored
        //     }
        //
        //     return new Type[] { };
        // }

        // public List<T> AcquireAny<T>(string directory)
        // {
        //     var loadedAssemblies = EnumerateAssemblies(directory);
        //
        //     var result = new List<T>();
        //
        //     var types = EnumerateTypes(loadedAssemblies, TypeCheck.InheritsFrom<T>);
        //
        //     foreach (var type in types)
        //     {
        //         var instance = TryInstantiate(type);
        //
        //         if (instance != null)
        //         {
        //             result.Add((T) instance);
        //         }
        //     }
        //
        //     Console.WriteLine($"{result.Count} Instances of {typeof(T).Name} has been found.");
        //
        //     return result;
        // }

        // private bool IsAlreadyLoaded(FileInfo file)
        // {
        //     var assemblyName = AssemblyName.GetAssemblyName(file.FullName);
        //
        //     var asses = AppDomain.CurrentDomain.GetAssemblies();
        //
        //     foreach (var assembly in asses)
        //     {
        //         if (assembly.FullName == assemblyName.FullName)
        //         {
        //             return true;
        //         }
        //     }
        //
        //     return false;
        // }

        // private List<Assembly> EnumerateAssemblies(string directory)
        // {
        //     var allDlls = EnumerateDlls(directory);
        //
        //     var result = new List<Assembly>();
        //
        //     foreach (var file in allDlls)
        //     {
        //         try
        //         {
        //             if (IsAlreadyLoaded(file))
        //             {
        //                 Console.WriteLine($"FILE: {file.Name} will be ignored cause assembly is already loaded.");
        //             }
        //             else
        //             {
        //                 var assembly = Assembly.LoadFrom(file.FullName);
        //
        //                 result.Add(assembly);
        //
        //                 Console.WriteLine("Loaded: " + file);
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             Console.WriteLine($"Did not load {file}, because: {ex.Message}");
        //         }
        //     }
        //
        //     return result;
        // }

        // private List<FileInfo> EnumerateDlls(string directory)
        // {
        //     var result = new List<FileInfo>();
        //
        //     var rootDirectory = new DirectoryInfo(directory);
        //
        //     EnumerateDlls(rootDirectory, result);
        //
        //     return result;
        // }

        // private void EnumerateDlls(DirectoryInfo directory, List<FileInfo> result)
        // {
        //     var files = directory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);
        //
        //     result.AddRange(files);
        //
        //     var directories = directory.EnumerateDirectories();
        //
        //     foreach (var directoryInfo in directories)
        //     {
        //         EnumerateDlls(directoryInfo, result);
        //     }
        // }

        private object TryInstantiate(Type type)
        {
            var constructor = type.GetConstructor(new Type[] { });

            if (constructor != null)
            {
                try
                {
                    var instance = constructor.Invoke(new object[] { });

                    if (instance != null)
                    {
                        return instance;
                    }
                }
                catch (Exception _)
                {
                    // ignored
                }
            }

            return null;
        }

        // public List<Type> EnumerateTypes(IEnumerable<Assembly> assemblies)
        // {
        //     return EnumerateTypes(assemblies, t => true);
        // }

        // public List<Type> EnumerateTypes(IEnumerable<Assembly> assemblies, Func<Type, bool> select)
        // {
        //     var result = new List<Type>();
        //
        //     foreach (var assembly in assemblies)
        //     {
        //         var types = new Type[] { };
        //
        //         try
        //         {
        //             types = assembly.GetTypes();
        //         }
        //         catch (Exception e)
        //         {
        //             Console.WriteLine($"Got no types out of {assembly.FullName} because of {e.Message}");
        //         }
        //
        //         foreach (var type in types)
        //         {
        //             if (select(type))
        //             {
        //                 result.Add(type);
        //             }
        //         }
        //     }
        //
        //
        //     Console.WriteLine($"** {result.Count} types has been loaded.");
        //
        //     return result;
        // }

        // public List<Type> EnumerateTypes(string directory)
        // {
        //     var assemblies = EnumerateAssemblies(directory);
        //
        //     var types = EnumerateTypes(assemblies);
        //
        //     return types;
        // }

        // public List<Type> EnumerateModels(string directory, string @namespace = "")
        // {
        //     var assemblies = EnumerateAssemblies(directory);
        //
        //     if (@namespace == null)
        //     {
        //         @namespace = "";
        //     }
        //
        //     var types = EnumerateTypes(assemblies, t =>
        //         TypeCheck.IsModel(t) &&
        //         t.Namespace != null &&
        //         t.Namespace.StartsWith(@namespace)
        //     );
        //
        //     return types;
        // }

        public List<T> AcquireAny<T>(List<Type> availableTypes)
        {
            var result = new List<T>();
            
            foreach (var type in availableTypes)
            {
                if (TypeCheck.InheritsFrom<T>(type))
                {
                    var instance = TryInstantiate(type);

                    if (instance != null)
                    {
                        result.Add((T) instance);
                    }
                }
            }
            return result;
        }

        public IEnumerable<Type> EnumerateModels(List<Type> availableTypes, string nameSpace)
        {
            return availableTypes.Where(t => TypeCheck.IsModel(t) && NamespaceMatch(nameSpace,t.Namespace));
        }

        private bool NamespaceMatch(string rootNs, string ns)
        {
            if (rootNs == null && ns == null)
            {
                return true;
            }

            if (rootNs == null)
            {
                return true;
            }

            if (ns == null)
            {
                return false;
            }

            if (rootNs.Length == 0)
            {
                return true;
            }

            if (ns.Length == 0)
            {
                return false;
            }

            return ns.StartsWith(rootNs);
        }
    }
}