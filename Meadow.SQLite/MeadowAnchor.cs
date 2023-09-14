using System.Reflection;

namespace Meadow.SQLite
{

    public static class MeadowSqLiteAnchorAssemblyExtensions
    {

        public static Assembly GetSqLiteMeadowAssembly(this Meadow.AnchorType a)
        {
            return typeof(MeadowSqLiteAnchorAssemblyExtensions).Assembly;
        }
    }

}