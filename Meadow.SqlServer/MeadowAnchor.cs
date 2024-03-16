using System.Reflection;

namespace Meadow.SqlServer
{

    public static class MeadowSqlServerAnchorAssemblyExtensions
    {

        public static Assembly GetSqlServerMeadowAssembly(this TheMeadow.AnchorType a)
        {
            return typeof(MeadowSqlServerAnchorAssemblyExtensions).Assembly;
        }
    }

}