
namespace Meadow.Scaffolding.Translators.Contracts.Translatabels.Select;


public class SelectTranslatable : ITranslatable
{
    public string Translate(int indent = 0)
    {
        throw new System.NotImplementedException();
    }
}



// using System;
// using System.Collections.Generic;
// using Meadow.Scaffolding.Models;
//
// namespace Meadow.Scaffolding.Translators.Contracts.Translatabels.Select;
//
//
// public class SelectTranslatable:ITranslatable
// {
//
//     private class Builder:IAlsoSelectBuilder,IWhereBuilder
//     {
//         public readonly List<Parameter> Parameters;
//
//         public Type FromType;
//         
//         public Builder(Parameter firstParameter)
//         {
//             Parameters = new List<Parameter>();
//             
//             Parameters.Add(firstParameter);
//         }
//
//         public IAlsoSelectBuilder AlsoSelect(Parameter parameter)
//         {
//             Parameters.Add(parameter);
//
//             return this;
//         }
//
//         public IWhereBuilder From(Type type)
//         {
//             FromType = type;
//
//             return this;
//         }
//
//         public void Where()
//         {
//             
//         }
//     }   
//     
//     
//
//     public IAlsoSelectBuilder Select(Parameter parameter)
//     {
//         var builder = new Builder(parameter);
//
//         return builder;
//     }
//     
//     
//     public string Translate(int indent = 0)
//     {
//
//         
//         throw new System.NotImplementedException();
//     }
// }