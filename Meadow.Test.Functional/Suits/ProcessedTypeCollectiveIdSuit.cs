using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Attributes;
using Meadow.Configuration;
using Meadow.Scaffolding.Models;
using Meadow.SQLite;
using Meadow.Utility;
using Xunit;
using Xunit.Sdk;

namespace Meadow.Test.Functional.Suits;


/// <summary>
/// TODO: CollectiveIdentificationProfile -> RecordIdentificationProfile
/// TODO:           .CollectiveIdentifiers: <string,List,FieldKey>  (Collection Attribute Name)
/// TODO:           .SingularIdentifiers   <string,FieldKey>        (UniqueMember Field Name)
/// 
/// TODO: SaveSnippet: Just Remove AG Fields From Inserts and Updates
/// </summary>
public class ProcessedTypeCollectiveIdSuit
{


    private class PoorModel
    {
        public int Prop1 { get; set; }
        
        public int Prop2 { get; set; }
        
        public int Prop3 { get; set; }
        
        public int Prop4 { get; set; }
    }
    
    private class AutoGeneratedIdModel
    {
        [AutoValuedMember]
        public int Id { get; set; }
        
        public int Prop1 { get; set; }
        
        public int Prop2 { get; set; }
        
        public int Prop3 { get; set; }
        
        public int Prop4 { get; set; }
    }
    
    private class CollectionRitchModel
    {
        [AutoValuedMember]
        public int Id { get; set; }
        
        [CollectiveIdentifier("FirstTwo","Odds")]
        public int Prop1 { get; set; }
        [CollectiveIdentifier("FirstTwo","Evens")]
        public int Prop2 { get; set; }
        [CollectiveIdentifier("SecondTwo","Odds")]
        public int Prop3 { get; set; }
        [CollectiveIdentifier("SecondTwo","Evens")]
        public int Prop4 { get; set; }
        
    }
    


    [Fact]
    public void Should_Create_A_ValuesOnlyProfile_For_PoorModel()
    {

        var processedType = EntityTypeUtilities.Process<PoorModel>(new MeadowConfiguration(),new SqLiteTypeNameMapper());

        
        AssertIsValueProfileOnly(processedType.RecordIdentificationProfile);

        AssertIsValidValuesProfile<PoorModel>(processedType.RecordIdentificationProfile.IdentifiersByCollectionName
            .First().Value);
    }



    [Fact]
    public void Should_Create_A_ValueProfile_For_AutoValuedIdModel() =>
        Should_Create_A_ValueProfile_For_Model<AutoGeneratedIdModel>();
    
    [Fact]
    public void Should_Create_A_ValueProfile_For_CollectionRitchModel() =>
        Should_Create_A_ValueProfile_For_Model<CollectionRitchModel>();


    [Fact]
    public void Should_Create_An_IdentifierProfile_For_AutoGeneratedIdModel()
        => Should_Create_An_IdentifierProfile_For_Model<AutoGeneratedIdModel>();
    
    [Fact]
    public void Should_Create_An_IdentifierProfile_For_CollectionRitchModel()
        => Should_Create_An_IdentifierProfile_For_Model<CollectionRitchModel>();
    
    
    public void Should_Create_A_ValueProfile_For_Model<T>()
    {

        var processedType = EntityTypeUtilities.Process<T>(new MeadowConfiguration(),new SqLiteTypeNameMapper());


        AssertHasOneValueProfile<T>(processedType.RecordIdentificationProfile);
    }
    
    public void Should_Create_An_IdentifierProfile_For_Model<T>()
    {

        var processedType = EntityTypeUtilities.Process<T>(new MeadowConfiguration(),new SqLiteTypeNameMapper());

        var profile = processedType.RecordIdentificationProfile;

        if (!profile.IdentifiersByCollectionName.ContainsKey(RecordIdentificationProfile.IdCollectionName))
        {
            throw new XunitException(
                $"Expected to find a profile item named: {RecordIdentificationProfile.IdCollectionName}," +
                $"but found none.");
        }

        AssertIsValidIdentifierProfile<T>(profile.IdentifiersByCollectionName
            [RecordIdentificationProfile.IdCollectionName]);
    }
    
    public void AssertHasOneValueProfile<T>(RecordIdentificationProfile profile)
    {

        if (!profile.IdentifiersByCollectionName.ContainsKey(RecordIdentificationProfile.DefaultCollection))
        {
            throw new XunitException("No ValuesOnly Profile Has been found.");
        }

        var p = profile.IdentifiersByCollectionName[RecordIdentificationProfile.DefaultCollection];
        
        AssertIsValidValuesProfile<T>(p);
    }

    private void AssertIsValidValuesProfile<TModel>(List<FieldKey> first)
    {
        var ev = new ObjectEvaluator(typeof(TModel));
        
        var propertyNames = ev.RootNode.GetDirectLeaves()
            .Select(l => ev.Map.FieldKeyByNode(l)).ToList();

        if (!ContainSameItems(first, propertyNames, (f1, f2) => f1.Equals(f2)))
        {
            var expected = string.Join("\n", propertyNames.Select(k => k.TerminalSegment().Name));
            var actual = string.Join("\n", first.Select(k => k.TerminalSegment().Name));

            throw new XunitException($"Expected Fields: \n{expected}\n" +
                                     $"Bu Received: \n{actual}");
        }
    }
    
    private void AssertIsValidIdentifierProfile<TModel>(List<FieldKey> first)
    {

        var idLeaf = TypeIdentity.GetAutoGeneratedLeaves<TModel>()
            ?.Select(l => l.GetFullName()).ToList() ?? new List<string>();

        var keys = first.Select(f => f.ToString()).ToList();
        
        if (!ContainSameItems(idLeaf, keys, (f1, f2) => f1.Equals(f2)))
        {
            var expected = string.Join("\n", idLeaf);
            var actual = string.Join("\n", first);

            throw new XunitException($"Expected Fields: \n{expected}\n" +
                                     $"Bu Received: \n{actual}");
        }
    }

    private bool ContainSameItems<T>(IEnumerable<T> values1, IEnumerable<T> values2, Func<T, T, bool> areEqual)
    {
        var list1 = values1.ToList();
        var list2 = values2.ToList();
        
        foreach (var value1 in list1)
        {
            if (list2.All(v2 => !areEqual(value1, v2)))
            {
                return false;
            }
        }

        if (list1.Count != list2.Count)
        {
            return false;
        }

        return true;
    }


    private void AssertIsValueProfileOnly(RecordIdentificationProfile profile)
    {
        if (profile.AutoValuedIdentifier)
        {
            throw new XunitException("Value Only Profile Should not have Auto-valued Identifier Fields");
        }

        if (profile.IdentifiersByCollectionName.Count != 1)
        {
            throw new XunitException("Value Only Profile Should Have exactly one collection.");
        }

        if (string.CompareOrdinal(profile.IdentifiersByCollectionName.First().Key ,
                RecordIdentificationProfile.DefaultCollection)!=0)
        {
            throw new XunitException($"ValueOnly profile's name must be {RecordIdentificationProfile.DefaultCollection}, " +
                                     $"But Created Profile is named: {profile.IdentifiersByCollectionName.First().Key}");
        }
    }
}