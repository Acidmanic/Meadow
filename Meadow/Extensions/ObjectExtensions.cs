using System;
using System.IO;
using System.Reflection;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
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

        public static TId? ReadIdOrDefault<TModel, TId>(this TModel value)
        {
            var idLeaf = TypeIdentity.FindIdentityLeaf<TModel>();

            if (idLeaf is { } idL)
            {
                // var readId = new ObjectEvaluator(value).Read(idL.GetFullName(), true);
                var readId = idL.Evaluator.Read(value);

                if (readId is TId id) return id;
            }

            return default;
        }
        
        
    }
}