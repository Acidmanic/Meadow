using System.Reflection;

namespace Meadow.Test.Unit;

public static class UnitTestAssemblyAnchor
{
    public static Assembly Assembly => typeof(UnitTestAssemblyAnchor).Assembly;
}