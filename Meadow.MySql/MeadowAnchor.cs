using System.Reflection;

namespace Meadow.MySql
{

    public static class MeadowMySqlAnchorAssemblyExtensions
    {

        public static Assembly GetMySqlMeadowAssembly(this Meadow.AnchorType a)
        {
            return typeof(MeadowMySqlAnchorAssemblyExtensions).Assembly;
        }
    }

}