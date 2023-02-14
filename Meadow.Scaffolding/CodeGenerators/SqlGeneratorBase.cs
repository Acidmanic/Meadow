using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Contracts;
using Meadow.DataTypeMapping;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.CodeGenerators
{
    public abstract class SqlGeneratorBase : ICodeGenerator
    {
        protected SqlGeneratorBase(IDbTypeNameMapper typeNameMapper)
        {
            TypeNameMapper = typeNameMapper;
        }

        public abstract Code Generate();


        public IDbTypeNameMapper TypeNameMapper { get; }


        protected void WalkThroughLeaves<TEntity>(bool fullTree, Action<AccessNode> leafAction)
        {
            WalkThroughLeaves(typeof(TEntity), fullTree, leafAction);
        }

        public void WalkThroughLeaves(Type type, bool fullTree, Action<AccessNode> leafAction)
        {
            var treeRoot = ObjectStructure.CreateStructure(type, true);

            var rootOnlyNode = ObjectStructure.CreateStructure(type, false);

            var node = fullTree ? treeRoot : rootOnlyNode;

            var info = new AccessTreeInformation(node);

            foreach (var leaf in info.OrderedLeaves)
            {
                leafAction(leaf);
            }
        }

        public List<Parameter> ToParameters<TEntity>()
        {
            return ToParameters(typeof(TEntity));
        }

        public List<Parameter> ToParameters(Type type)
        {
            var rootOnlyNode = ObjectStructure.CreateStructure(type, false);

            var children = rootOnlyNode.GetChildren();

            var parameters = new List<Parameter>();
            
            foreach (var child in children)
            {
                if (child.IsLeaf)
                {
                    var typeName = TypeNameMapper[child.Type];
                    
                    parameters.Add(new Parameter
                    {
                         Name = child.Name,
                         Type = typeName
                    });
                }
            }

            return parameters;
        }

        public ProcessedType Process<TEntity>()
        {
            return Process(typeof(TEntity));
        }
        
        public ProcessedType Process(Type type)
        {
            var process = new ProcessedType
            {
                Parameters = ToParameters(type),
                NameConvention = new NameConvention(type),
                IdField = TypeIdentity.FindIdentityLeaf(type),
                HasId = false
            };

            process.NoneIdParameters = new List<Parameter>();
            
            foreach (var parameter in process.Parameters)
            {
                if (parameter.Name == process.IdField.Name)
                {
                    process.HasId = true;

                    process.IdParameter = parameter;
                }
                else
                {
                    process.NoneIdParameters.Add(parameter);
                }
            }

            return process;
        }
        
        
    }
}