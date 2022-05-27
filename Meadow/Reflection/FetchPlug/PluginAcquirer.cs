using System;
using System.Collections.Generic;
using System.Reflection;

namespace Meadow.Reflection.FetchPlug
{
    public class PluginAcquirer
    {
        public List<T> AcquireAny<T>(Assembly assembly)
        {
            var result = new List<T>();

            foreach (var module in assembly.GetModules())
            {
                var types = module.GetTypes();

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

        public List<T> AcquireAny<T>()
        {
            var result = new List<T>();

            result.AddRange(AcquireAny<T>(Assembly.GetCallingAssembly()));

            result.AddRange(AcquireAny<T>(Assembly.GetExecutingAssembly()));

            return result;
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