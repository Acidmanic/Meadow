namespace Meadow.Scaffolding.Macros;



/// <summary>
/// This enumerator represents three different modes for an external tool to call for applying macros.
/// <ul>
/// <li><b>The ForceFull mode:</b> which can replace any macros (<i>by-force</i>, <i>regular</i> and <i>always</i>).</li>
/// <li><b>The Regular mode:</b> which can replace <i>regular</i> and <i>always</i> macros.</li>
/// <li><b>The Weak mode:</b> which only can replace<i>always</i> macros.</li>
/// </ul>
/// </summary>
public enum ExternalToolCallMode
{
    Regular,
    ForceFul,
    Weak
}