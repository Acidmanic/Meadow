using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        protected void WalkThroughLeaves(Type type, Action<AccessNode> scan)
        {
            var rootOnlyNode = ObjectStructure.CreateStructure(type, false);

            var children = rootOnlyNode.GetChildren();

            foreach (var child in children)
            {
                if (child.IsLeaf)
                {
                    scan(child);
                }
            }
        }


        public List<Parameter> ToParameters(Type type)
        {
            var parameters = new List<Parameter>();

            WalkThroughLeaves(type,
                l =>
                {
                    var typeName = TypeNameMapper[l.Type];

                    parameters.Add(new Parameter
                    {
                        Name = l.Name,
                        Type = typeName
                    });
                });

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
                NameConvention = new NameConvention(type),
                IdField = TypeIdentity.FindIdentityLeaf(type),
                HasId = false
            };
            process.Parameters = new List<Parameter>();
            process.NoneIdParameters = new List<Parameter>();
            process.NoneIdUniqueParameters = new List<Parameter>();

            WalkThroughLeaves(type, leaf =>
            {
                var parameter = new Parameter
                {
                    Name = leaf.Name,
                    Type = TypeNameMapper[leaf.Type]
                };
                
                process.Parameters.Add(parameter);
                
                if (leaf.Name == process.IdField.Name)
                {
                    process.HasId = true;

                    process.IdParameter = parameter;                
                }
                else
                {
                    process.NoneIdParameters.Add(parameter);

                    if (leaf.IsUnique)
                    {
                        process.NoneIdUniqueParameters.Add(parameter);
                    }
                }
            });
     
            return process;
        }

        protected string SqlProcedureDeclaration(Parameter p, string prefix = "")
        {
            return prefix + p.Name + " " + p.Type;
        }
    }
}