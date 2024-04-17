using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.FieldInclusion;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Configuration;
using Meadow.DataTypeMapping;
using Meadow.DataTypeMapping.Attributes;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Models;

namespace Meadow.Utility;

public static class EntityTypeUtilities
{
    public static readonly int IndexCorpusSize = 4000;

    public static void WalkThroughLeaves<TEntity>(bool fullTree, Action<AccessNode> leafAction)
    {
        WalkThroughLeaves(typeof(TEntity), fullTree, leafAction);
    }

    public static void WalkThroughLeaves(Type type, bool fullTree, Action<AccessNode> leafAction)
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


    public static void WalkThroughLeaves(Type type, Action<AccessNode> scan)
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

    private static AccessNode SearchIndexCorpusLeaf()
    {
        var rootOnlyNode = ObjectStructure.CreateStructure(typeof(SearchIndex<object>), false);

        return rootOnlyNode.GetChildren().First(c => c.Name == nameof(SearchIndex<object>.IndexCorpus));
    }

    public static ProcessedType Process<TEntity>(MeadowConfiguration configuration, IDbTypeNameMapper typeNameMapper)
    {
        return Process(typeof(TEntity), configuration, typeNameMapper);
    }

    public static string GetTypeName(
        AccessNode leaf,
        MeadowConfiguration configuration,
        IDbTypeNameMapper typeNameMapper)
    {
        var searchIndexCorpusAddress = MemberOwnerUtilities.GetAddress<SearchIndex<object>, string>
            (s => s.IndexCorpus);

        var searchIndexCorpusColumnSize = 4000;

        var sizesByAddress = new Dictionary<string, int>();

        foreach (var item in configuration.ExternallyForcedColumnSizesByNodeAddress)
        {
            sizesByAddress.Add(item.Key.ToLower(), item.Value);
        }

        sizesByAddress.Add(searchIndexCorpusAddress.ToLower(), searchIndexCorpusColumnSize);

        var leafKey = leaf.GetFullName().ToLower();

        var propertyAttributes = new List<Attribute>(leaf.PropertyAttributes);

        if (sizesByAddress.ContainsKey(leafKey))
        {
            var leafColumnSize = sizesByAddress[leafKey];

            propertyAttributes.Add(new ForceColumnSizeAttribute(leafColumnSize));
        }

        var typeName = typeNameMapper.GetDatabaseTypeName(leaf.Type.GetAlteredOrOriginalType(), propertyAttributes);

        return typeName;
    }

    public static ProcessedType Process(Type type,
        MeadowConfiguration configuration,
        IDbTypeNameMapper typeNameMapper,
        IFieldInclusion? inclusions = null)
    {
        var inc = inclusions ?? (IFieldInclusion)(new FiledManipulationMarker());

        var process = new ProcessedType
        {
            NameConvention = configuration.GetNameConvention(type),
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

        var indexCorpusLeaf = SearchIndexCorpusLeaf();

        process.IndexCorpusParameter = new Parameter
        {
            Name = indexCorpusLeaf.Name,
            Type = GetTypeName(indexCorpusLeaf, configuration, typeNameMapper)
        };

        var fullTreeMap = new FullTreeMap(type,
            configuration.DatabaseFieldNameDelimiter,
            configuration.TableNameProvider);

        WalkThroughLeaves(type, leaf =>
        {
            var key = fullTreeMap.Evaluator.Map.FieldKeyByNode(leaf);

            if (inc.IsIncluded(key))
            {
                var parameter = new Parameter
                {
                    Name = leaf.Name,
                    Type = GetTypeName(leaf, configuration, typeNameMapper)
                };
                var parameterFullTree = new Parameter
                {
                    Type = parameter.Type,
                    Name = fullTreeMap.GetColumnNameByFullAddress(leaf.GetFullName())
                };

                process.Parameters.Add(parameter);
                process.ParametersFullTree.Add(parameterFullTree);

                if (process.HasId && leaf.Name == process.IdField.Name)
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
            }
        });

        var foundAsEventStream = EventStreamInfo.FromType(type);

        process.IsStreamEvent = foundAsEventStream.Success;

        if (foundAsEventStream)
        {
            process.EventStream = foundAsEventStream;
            process.EventIdTypeName = typeNameMapper.GetDatabaseTypeName(foundAsEventStream.Value.EventIdType);
            process.StreamIdTypeName = typeNameMapper.GetDatabaseTypeName(foundAsEventStream.Value.StreamIdType);
            process.EventStreamTypeNameDatabaseType = typeNameMapper.GetDatabaseTypeNameForString
                (foundAsEventStream.Value.MaximumTypeNameLength);
            process.EventStreamSerializedValueDatabaseType = typeNameMapper.GetDatabaseTypeNameForString
                (foundAsEventStream.Value.MaximumDataSize);
            process.IsEventIdAutogenerated = IsNumeric(foundAsEventStream.Value.EventIdType);
        }

        return process;
    }

    public static bool IsNumeric(Type type)
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