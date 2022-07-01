using System.Collections.Generic;
using System.IO;

namespace Meadow.Test.Functional
{
    public class BinaryResources
    {
        public static byte[] Read(string name)
        {
            var file = "BinaryResources";

            file = Path.Join(file,name + ".b64");

            if (!File.Exists(file))
            {
                return default;
            }

            var b64 = File.ReadAllText(file);

            return System.Convert.FromBase64String(b64);
        }
    }
}