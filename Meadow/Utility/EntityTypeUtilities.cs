using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Filtering.Models;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Acidmanic.Utilities.Results;
using Meadow.Attributes;
using Meadow.Configuration;
using Meadow.Contracts;
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
        var rootOnlyNode = new ObjectEvaluator(type).RootNode;

        var children = rootOnlyNode.GetDirectLeaves(true);

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

        var typeName = typeNameMapper.GetDatabaseTypeName(leaf.Type.GetAlteredOrOriginal(), propertyAttributes);

        return typeName;
    }

    public static ProcessedType Process(Type type, MeadowConfiguration configuration, IDbTypeNameMapper typeNameMapper)
    {
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
            var parameter = new Parameter
            {
                Name = leaf.Name,
                StandardAddress = leaf.GetFullName(),
                IdentifierStatus = GetIdentifierStatus(leaf),
                IsNumerical = TypeCheck.IsNumerical(leaf.Type),
                IsString = leaf.Type == typeof(string) || leaf.Type ==  typeof(Guid),
                Type = GetTypeName(leaf, configuration, typeNameMapper)
            };
            var parameterFullTree = new Parameter
            {
                Type = parameter.Type,
                StandardAddress = parameter.StandardAddress,
                IdentifierStatus = parameter.IdentifierStatus,
                IsNumerical = parameter.IsNumerical,
                IsString = parameter.IsString,
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
        });

        var foundAsEventStream = EventStreamPreferencesInfo.FromType(type);

        process.IsStreamEvent = foundAsEventStream.Success;

        if (foundAsEventStream)
        {
            process.EventStream = foundAsEventStream;
            process.EventIdType = foundAsEventStream.Value.EventIdType;
            process.EventIdTypeName = typeNameMapper.GetDatabaseTypeName(foundAsEventStream.Value.EventIdType);
            process.StreamIdTypeName = typeNameMapper.GetDatabaseTypeName(foundAsEventStream.Value.StreamIdType);
            process.EventStreamTypeNameDatabaseType = typeNameMapper.GetDatabaseTypeNameForString
                (foundAsEventStream.Value.MaximumTypeNameLength);
            process.EventStreamAssemblyNameDatabaseType = typeNameMapper.GetDatabaseTypeNameForString
                (foundAsEventStream.Value.MaximumTypeNameLength);

            process.EventStreamSerializedValueDatabaseType = typeNameMapper.GetDatabaseTypeNameForString
                (foundAsEventStream.Value.MaximumDataSize);
            process.IsEventIdAutogenerated = IsNumeric(foundAsEventStream.Value.EventIdType);
        }

        process.RecordIdentificationProfile = GetCollectiveIdentificationProfile(type);

        process.SaveProcedureNames = process.NameConvention.GetSaveProcedureNames(process.RecordIdentificationProfile);

        return process;
    }

    private static ParameterIdentifierStatus GetIdentifierStatus(AccessNode leaf)
    {
        ParameterIdentifierStatus status = ParameterIdentifierStatus.None;

        if (leaf.IsUnique) status |= ParameterIdentifierStatus.Unique;

        if (leaf.IsAutoValued) status |= ParameterIdentifierStatus.AutoGeneratedUnique;

        return status;
    }

    public static RecordIdentificationProfile GetCollectiveIdentificationProfile(Type type)
    {
        var profile = new RecordIdentificationProfile();

        var ev = new ObjectEvaluator(type);

        var leaves = ev.RootNode.GetDirectLeaves(true);

        foreach (var leaf in leaves)
        {
            var fieldKey = ev.Map.FieldKeyByNode(leaf);

            if (leaf.IsUnique || leaf.IsAutoValued)
            {
                profile.AddSingularIdentifierItem(leaf.Name, fieldKey);
            }
            else if (leaf.PropertyAttributes.FirstOrDefault(at => at is CollectiveIdentifierAttribute) is
                     CollectiveIdentifierAttribute collectiveIdentifier)
            {
                foreach (var collectionName in collectiveIdentifier.CollectionNames)
                {
                    profile.AddCollectiveIdentifierItem(collectionName, fieldKey);
                }
            }

            profile.AddCollectiveIdentifierItem(RecordIdentificationProfile.DefaultCollection, fieldKey);
        }

        return profile;
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