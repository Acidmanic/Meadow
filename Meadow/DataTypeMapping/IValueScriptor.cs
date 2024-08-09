namespace Meadow.DataTypeMapping;

/// <summary>
/// This will be used when an object needs to be put inside sql script as string. which will be different in
/// various database systems.It would not be needed for normal Ado usage because Ado does that automatically
/// using <dbName>Command objects. But to create dynamic script portions like macros end etc.. it will be needed
/// </summary>
public interface IValueScriptor
{


    string Translate(object value);
}