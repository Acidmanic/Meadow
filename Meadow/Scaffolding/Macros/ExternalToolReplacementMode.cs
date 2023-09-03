namespace Meadow.Scaffolding.Macros;

/// <summary>
/// This values are representing the existence of "BY-FORCE" or "ALWAYS" markers on the macro declaration.
/// If A macro is not marked by any, it would be a regular macro.  
/// </summary>
public enum ExternalToolReplacementMode
{
    /// <summary>
    /// The default mode: Macro can be replaced by regular call or by force, but not with a weak call 
    /// </summary>
    Regular,
    /// <summary>
    /// Macro can only be replaced by a forceful call. Regular calls and weak calls can not replace the macro. 
    /// </summary>
    ByForce,
    /// <summary>
    /// Any call can replace the macro.
    /// </summary>
    Always
}