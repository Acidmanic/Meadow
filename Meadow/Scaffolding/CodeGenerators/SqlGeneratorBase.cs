using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.CodeGenerators
{
    public abstract class SqlGeneratorBase : ICodeGenerator
    {
        protected SqlGeneratorBase(IDbTypeNameMapper typeNameMapper,MeadowConfiguration configuration)
        {
            TypeNameMapper = typeNameMapper;
            Configuration = configuration;
        }

        public RepetitionHandling RepetitionHandling { get; set; } = RepetitionHandling.Create;
        
        public abstract Code Generate();


        protected MeadowConfiguration Configuration { get; }

        public IDbTypeNameMapper TypeNameMapper { get; }


        protected bool IsDatabaseObjectNameForced { get; private set; }
        
        protected string ForcedDatabaseObjectName { get; private set; }

        protected void WalkThroughLeaves<TEntity>(bool fullTree, Action<AccessNode> leafAction)
        {
            WalkThroughLeaves(typeof(TEntity), fullTree, leafAction);
        }

        public SqlGeneratorBase ForceDatabaseObjectName(string forcedName)
        {
            IsDatabaseObjectNameForced = true;

            ForcedDatabaseObjectName = forcedName;

            return this;
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

        public ProcessedType Process<TEntity>()
        {
            return Process(typeof(TEntity));
        }

        public ProcessedType Process(Type type)
        {
            var process = new ProcessedType
            {
                NameConvention = Configuration.GetNameConvention(type),
                IdField = TypeIdentity.FindIdentityLeaf(type),
                HasId = false
            };
            process.HasId = process.IdField != null;
            process.Parameters = new List<Parameter>();
            process.NoneIdParameters = new List<Parameter>();
            process.NoneIdUniqueParameters = new List<Parameter>();
            
            process.ParametersFullTree = new List<Parameter>();
            process.NoneIdParametersFullTree = new List<Parameter>();
            process.NoneIdUniqueParametersFullTree = new List<Parameter>();

            var fullTreeMap = new FullTreeMap(type,
                Configuration.DatabaseFieldNameDelimiter,
                Configuration.TableNameProvider);
            
            WalkThroughLeaves(type, leaf =>
            {
                var parameter = new Parameter
                {
                    Name = leaf.Name,
                    Type = TypeNameMapper.GetDatabaseTypeName(leaf.Type.GetAlteredOrOriginalType(), leaf.PropertyAttributes)
                };
                var parameterFullTree = new Parameter
                {
                    Type = parameter.Type,
                    Name = fullTreeMap.GetColumnNameByFullAddress(leaf.GetFullName())
                };

                process.Parameters.Add(parameter);
                process.ParametersFullTree.Add(parameterFullTree);

                if (process.HasId &&  leaf.Name == process.IdField.Name)
                {
                    process.IdParameter = parameter;
                    process.IdParameterFullTree = parameterFullTree;
                }
                else
                {
                    process.NoneIdParameters.Add(parameter);
                    process.NoneIdParametersFullTree.Add(parameterFullTree);

                    if (leaf.IsUnique)
                    {
                        process.NoneIdUniqueParameters.Add(parameter);
                        process.NoneIdUniqueParametersFullTree.Add(parameterFullTree);
                    }
                }
            });

            var foundAsEventStream = EventStreamInfo.FromType(type);

            process.IsStreamEvent = foundAsEventStream.Success;

            if (foundAsEventStream)
            {
                process.EventStream = foundAsEventStream;
                process.EventIdTypeName = TypeNameMapper.GetDatabaseTypeName(foundAsEventStream.Value.EventIdType);
                process.StreamIdTypeName = TypeNameMapper.GetDatabaseTypeName(foundAsEventStream.Value.StreamIdType);
                process.EventStreamTypeNameDatabaseType = TypeNameMapper.GetDatabaseTypeNameForString
                    (foundAsEventStream.Value.MaximumTypeNameLength);
                process.EventStreamSerializedValueDatabaseType = TypeNameMapper.GetDatabaseTypeNameForString
                    (foundAsEventStream.Value.MaximumDataSize);
                process.IsEventIdAutogenerated = IsNumeric(foundAsEventStream.Value.EventIdType);
            }
            
            return process;
        }

        protected string ParameterNameTypeJoint(Parameter p, string namePrefix = "")
        {
            return namePrefix + p.Name + " " + p.Type;
        }
        
        protected string ParameterNameValueSetJoint(Parameter p, string valuePrefix = "")
        {
            return p.Name + " = " + valuePrefix + p.Name;
        }

        protected string ParameterNameTypeJoint(IEnumerable<Parameter> parameters, string delimiter, string namePrefix = "")
        {
            return string.Join(delimiter, parameters.Select(p => ParameterNameTypeJoint(p, namePrefix)));
        }
        
        protected string ParameterNameValueSetJoint(IEnumerable<Parameter> parameters, string delimiter, string valuePrefix = "")
        {
            return string.Join(delimiter, parameters.Select(p => ParameterNameValueSetJoint(p, valuePrefix)));
        }

        protected bool IsNumeric(Type type)
        {
            return type == typeof(long) ||
                   type == typeof(int) ||
                   type == typeof(short) ||
                   type == typeof(byte) ||
                   type == typeof(double) ||
                   type == typeof(float) ||
                   type == typeof(decimal) ||
                   type == typeof(sbyte) ||
                   type == typeof(uint) ||
                   type == typeof(ushort) ||
                   type == typeof(ulong);
        }
    }
}