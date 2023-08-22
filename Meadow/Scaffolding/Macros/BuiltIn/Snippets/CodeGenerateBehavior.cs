using System;

namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets;

[Flags]
public enum CodeGenerateBehavior
{
    UseById = 1,
    UseAll = 2,
    UseIdAware = UseById | UseAll,
    UseIdAgnostic = 4,
    UseEveryMethod = UseIdAware | UseIdAgnostic,
    UseNone = 0
}