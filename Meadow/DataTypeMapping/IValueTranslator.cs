// using System;
//
// namespace Meadow.DataTypeMapping;
//
// /// <summary>
// /// This will be used when an object needs to be put inside sql script as string. which will be different in
// /// various database systems.It would not be needed for normal Ado usage because Ado does that automatically
// /// using <dbName>Command objects. But to create dynamic script portions like macros end etc.. it will be needed
// /// </summary>
// public interface IValueTranslator
// {
//
//     public static readonly IValueTranslator Null = new NullValueTranslator();
//
//     string Translate(object? value);
//     
//     private class NullValueTranslator:IValueTranslator
//     {
//         public string Translate(object? value) =>  $"{value ?? string.Empty}";
//         public string Quote(Type _, string value) => value;
//     }
// }