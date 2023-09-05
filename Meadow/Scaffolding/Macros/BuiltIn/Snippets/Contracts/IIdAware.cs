namespace Meadow.Scaffolding.Macros.BuiltIn.Snippets.Contracts;

/// <summary>
/// An IdAware class, is a class that can behave in two different manners: 1) ById, 2) All
/// </summary>
public interface IIdAware
{
    
    /// <summary>
    /// This property determine whether the object performs it's task in "ById" manner or "All" manner.
    /// </summary>
    public bool ActById { get; set; }
}