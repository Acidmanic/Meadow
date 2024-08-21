using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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


public class ProcessedTypeCollectiveIdSuit
{

    private class Marker
    {
        public List<FieldKey> Keys { get; } = new List<FieldKey>();

        public void Mark<TM, TP>(Expression<Func<TM, TP>> select)
        {
            var key = MemberOwnerUtilities.GetKey(select);
            
            if(!Keys.Contains(key)) Keys.Add(key);
        } 
        
    }

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

    private record CollectionRitchRecord([AutoValuedMember] int Id, [UniqueMember] int SystemId, [CollectiveIdentifier("Group1")] int Prop1, [CollectiveIdentifier("Group1")] int Prop2);
    


    [Fact]
    public void Should_Create_A_ValuesOnlyProfile_For_PoorModel()
    {

        var processedType = EntityTypeUtilities.Process<PoorModel>(new MeadowConfiguration(),new SqLiteTypeNameMapper());

        
        AssertIsValueProfileOnly(processedType.RecordIdentificationProfile);

        AssertIsValidValuesProfile<PoorModel>(processedType.RecordIdentificationProfile.CollectiveIdentifiersByName
            .First().Value);
    }

    [Fact]
    public void Should_Create_A_SingularSet_Named_Id_For_CollectionRitchRecord()
    {
        Should_Create_IdentifierSet<CollectionRitchRecord>(false,"Id", m =>
        {
            m.Mark((CollectionRitchRecord r)=> r.Id);
        });
    }
    
    [Fact]
    public void Should_Create_A_SingularSet_Named_SystemId_For_CollectionRitchRecord()
    {
        Should_Create_IdentifierSet<CollectionRitchRecord>(false,"SystemId", m =>
        {
            m.Mark((CollectionRitchRecord r)=> r.SystemId);
        });
    }
    
    [Fact]
    public void Should_Create_A_CollectiveSet_Named_Group1_For_CollectionRitchRecord()
    {
        Should_Create_IdentifierSet<CollectionRitchRecord>(true,"Group1", m =>
        {
            m.Mark((CollectionRitchRecord r)=> r.Prop1);
            m.Mark((CollectionRitchRecord r)=> r.Prop2);
        });
    }


    [Fact]
    public void Should_Create_A_ValueProfile_For_AutoValuedIdModel() =>
        Should_Create_A_ValueProfile_For_Model<AutoGeneratedIdModel>();
    
    [Fact]
    public void Should_Create_A_ValueProfile_For_CollectionRitchModel() =>
        Should_Create_A_ValueProfile_For_Model<CollectionRitchModel>();

    [Fact]
    public void Should_Create_Id_IdentifierSet_For_AutoGeneratedIdModel()
    {
        Should_Create_IdentifierSet<AutoGeneratedIdModel>(false,nameof(AutoGeneratedIdModel.Id),
            marker =>
            {
                marker.Mark((AutoGeneratedIdModel m) => m.Id );
            });
    }
    
    [Fact]
    public void Should_Create_Id_IdentifierSet_For_CollectionRitchModel()
    {
        Should_Create_IdentifierSet<CollectionRitchModel>(false,nameof(CollectionRitchModel.Id),
            marker =>
            {
                marker.Mark((CollectionRitchModel m) => m.Id );
            });
    }

    
    [Fact]
    public void Should_Create_FirstTwo_IdentifierSet_For_CollectionRitchModel()
    {
        Should_Create_IdentifierSet<CollectionRitchModel>(true,"FirstTwo",
            marker =>
            {
                marker.Mark((CollectionRitchModel m) => m.Prop1 );
                marker.Mark((CollectionRitchModel m) => m.Prop2 );
            });
    }
    
    [Fact]
    public void Should_Create_SecondTwo_IdentifierSet_For_CollectionRitchModel()
    {
        Should_Create_IdentifierSet<CollectionRitchModel>(true,"SecondTwo",
            marker =>
            {
                marker.Mark((CollectionRitchModel m) => m.Prop3 );
                marker.Mark((CollectionRitchModel m) => m.Prop4 );
            });
    }
    
    [Fact]
    public void Should_Create_Evens_IdentifierSet_For_CollectionRitchModel()
    {
        Should_Create_IdentifierSet<CollectionRitchModel>(true,"Evens",
            marker =>
            {
                marker.Mark((CollectionRitchModel m) => m.Prop2 );
                marker.Mark((CollectionRitchModel m) => m.Prop4 );
            });
    }
    
    [Fact]
    public void Should_Create_Odds_IdentifierSet_For_CollectionRitchModel()
    {
        Should_Create_IdentifierSet<CollectionRitchModel>(true,"Odds",
            marker =>
            {
                marker.Mark((CollectionRitchModel m) => m.Prop1 );
                marker.Mark((CollectionRitchModel m) => m.Prop3 );
            });
    }

    
    public void Should_Create_A_ValueProfile_For_Model<T>()
    {

        var processedType = EntityTypeUtilities.Process<T>(new MeadowConfiguration(),new SqLiteTypeNameMapper());


        AssertHasOneValueProfile<T>(processedType.RecordIdentificationProfile);
    }



    private void Should_Create_IdentifierSet<T>(bool collective, string name, Action<Marker> mark)
    {
        var processedType = EntityTypeUtilities.Process<T>(new MeadowConfiguration(),new SqLiteTypeNameMapper());

        FieldKey[] fieldKeys;
        
        if (collective)
        {
            if (!processedType.RecordIdentificationProfile.CollectiveIdentifiersByName.ContainsKey(name))
            {
                throw new XunitException($"Expected to contain identifier set named {name}, It it was not found.");
            }

            fieldKeys = processedType.RecordIdentificationProfile.CollectiveIdentifiersByName[name].ToArray();
        }
        else
        {
            if (!processedType.RecordIdentificationProfile.SingularIdentifiersByName.ContainsKey(name))
            {
                throw new XunitException($"Expected to contain identifier set named {name}, It it was not found.");
            }

            fieldKeys = new []{processedType.RecordIdentificationProfile.SingularIdentifiersByName[name]};
        }

        AssertIsValidIdentifierKeys<T>(mark, fieldKeys);
    }
    
    
    public void AssertHasOneValueProfile<T>(RecordIdentificationProfile profile)
    {

        if (!profile.CollectiveIdentifiersByName.ContainsKey(RecordIdentificationProfile.DefaultCollection))
        {
            throw new XunitException("No ValuesOnly Profile Has been found.");
        }

        var p = profile.CollectiveIdentifiersByName[RecordIdentificationProfile.DefaultCollection];
        
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


    private void AssertIsValidIdentifierKeys<TModel>(Action<Marker> markExpected,params FieldKey[] idSetFields)
    {
        var marker = new Marker();

        markExpected(marker);

        AssertSameFieldsSet(marker.Keys, idSetFields.ToList());

    }
    
    private void AssertSameFieldsSet(List<FieldKey> expectedFields,List<FieldKey> actualFields)
    {
        
        if (!ContainSameItems(expectedFields, actualFields, (f1, f2) => f1.Equals(f2)))
        {
            var expected = string.Join("\n", expectedFields.Select(k => k.TerminalSegment().Name));
            var actual = string.Join("\n", actualFields.Select(k => k.TerminalSegment().Name));

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
       
        if (profile.CollectiveIdentifiersByName.Count != 1)
        {
            throw new XunitException("Value Only Profile Should Have exactly one collection.");
        }
        
        if (profile.SingularIdentifiersByName.Count != 0)
        {
            throw new XunitException("Value Only Profiles Can not have singular identifiers.");
        }

        if (string.CompareOrdinal(profile.CollectiveIdentifiersByName.First().Key ,
                RecordIdentificationProfile.DefaultCollection)!=0)
        {
            throw new XunitException($"ValueOnly profile's name must be {RecordIdentificationProfile.DefaultCollection}, " +
                                     $"But Created Profile is named: {profile.CollectiveIdentifiersByName.First().Key}");
        }
    }
}