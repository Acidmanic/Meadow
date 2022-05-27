using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Meadow.Reflection.FetchPlug
{
    public class PluginAcquirer
    {
        public List<T> AcquireAny<T>(Assembly assembly)
        {
            var result = new List<T>();

            foreach (var module in assembly.GetModules())
            {

                var types = SafeGetModuleTypes(module);

                foreach (var type in types)
                {
                    if (TypeCheck.InheritsFrom<T>(type))
                    {
                        var obj = TryInstantiate(type);

                        if (obj != null)
                        {
                            result.Add((T) obj);
                        }
                    }
                }
            }

            return result;
        }

        private IEnumerable<Type> SafeGetModuleTypes(Module module)
        {
            try
            {
                return module.GetTypes();
            }
            catch (Exception _)
            {
                // ignored
            }

            return new Type[] { };
        }

        public List<T> AcquireAny<T>()
        {
            var result = new List<T>();

            result.AddRange(AcquireAny<T>(Assembly.GetCallingAssembly()));

            result.AddRange(AcquireAny<T>(Assembly.GetExecutingAssembly()));

            return result;
        }

        public List<T> AcquireAny<T>(string directory)
        {
            var loadedAssemblies = EnumerateAssemblies(directory);

            var result = new List<T>();

            loadedAssemblies.ForEach(ass =>
                result.AddRange(AcquireAny<T>(ass))
            );

            return result;
        }

        private List<Assembly> EnumerateAssemblies(string directory)
        {
            var allDlls = EnumerateDlls(directory);

            var result = new List<Assembly>();

            foreach (var file in allDlls)
            {
                try
                {
                    var assembly = Assembly.LoadFile(file.FullName);

                    result.Add(assembly);
                }
                catch (Exception _)
                {
                    // ignored
                }
            }

            return result;
        }

        private List<FileInfo> EnumerateDlls(string directory)
        {
            var result = new List<FileInfo>();

            var rootDirectory = new DirectoryInfo(directory);

            EnumerateDlls(rootDirectory, result);

            return result;
        }

        private void EnumerateDlls(DirectoryInfo directory, List<FileInfo> result)
        {
            var files = directory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);

            result.AddRange(files);

            var directories = directory.EnumerateDirectories();

            foreach (var directoryInfo in directories)
            {
                EnumerateDlls(directoryInfo, result);
            }
        }

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
    }
}