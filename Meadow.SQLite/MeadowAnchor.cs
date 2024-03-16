using System.Reflection;

namespace Meadow.SQLite
{

    public static class MeadowSqLiteAnchorAssemblyExtensions
    {

        public static Assembly GetSqLiteMeadowAssembly(this TheMeadow.AnchorType a)
        {
            return typeof(MeadowSqLiteAnchorAssemblyExtensions).Assembly;
        }
    }

}