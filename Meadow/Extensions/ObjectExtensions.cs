using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Meadow.Extensions
{
    public static class ObjectExtensions
    {
        public static void SaveBesideAssembly(this object value, string title = "")
        {
            var name = DateTime.Now.ToString("yyyyMMddhhmmss") + title + ".json";

            var path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory?.FullName;

            path = Path.Join(path, name);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var json = JsonConvert.SerializeObject(value);

            File.WriteAllText(path, json);
        }


        public static T ReadJsonBesideAssembly<T>(this object value, string name)
        {
            var path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory?.FullName;

            path = Path.Join(path, name);

            var json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<T>(json);

        }
        
        
    }
}