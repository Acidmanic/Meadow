using System.Reflection;

namespace Meadow.Postgre
{

    public static class MeadowPostgreAnchorAssemblyExtensions
    {

        public static Assembly GetPostgreMeadowAssembly(this Meadow.AnchorType a)
        {
            return typeof(MeadowPostgreAnchorAssemblyExtensions).Assembly;
        }
    }

}