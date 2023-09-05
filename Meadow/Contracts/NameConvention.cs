using System;
using System.Reflection;
using Acidmanic.Utilities.Reflection.Attributes;
using Acidmanic.Utilities.Reflection.ObjectTree;
using CorePluralizer.Extensions;

namespace Meadow.Contracts
{
    public class NameConvention
    {
        public string EntityName { get; private set; }

        public string TableName { get; private set; }
        
        public string FilterResultsTableName { get;  }
        
        public string SearchIndexTableName { get;  }

        public IDataOwnerNameProvider TableNameProvider { get; }
        
        public Type EntityType { get; private set; }
        
        /* Event Stream related */

        public string EventStreamEntity { get; set; }
        
        public string EventStreamTableName { get; private set; }
        
        public string InsertEvent { get; private set; }
        
        public string ReadAllStreams { get; private set; }
        
        public string ReadStreamByStreamId { get; private set; }
        
        public string ReadAllStreamsChunks { get; private set; }
        
        public string ReadStreamChunkByStreamId { get; private set; }
        
        public string RangeProcedureName { get; private set; }
        
        public string ExistingValuesProcedureName { get; private set; }
        
        public string FullTreeViewName { get; private set; }
        

        public NameConvention(Type entityType) : this(entityType, new PluralDataOwnerNameProvider())
        {
        }

        public NameConvention(Type entityType, IDataOwnerNameProvider tableNameProvider)
        {
            EntityType = entityType;

            EntityName = EntityType.Name;

            TableNameProvider = tableNameProvider;

            TableName = TableNameProvider.GetNameForOwnerType(EntityType);
            
            FullTreeViewName = TableName + "FullTree";

            DeleteAllProcedureName = "spDeleteAll" + TableName;

            DeleteByIdProcedureName = "spDelete" + EntityName + "ById";

            SelectByIdProcedureName = "spRead" + EntityName + "ById";

            SelectByIdProcedureNameFullTree = "spRead" + EntityName + "ByIdFullTree";

            SelectAllProcedureName = "spReadAll" + TableName;

            SelectAllProcedureNameFullTree = "spReadAll" + TableName + "FullTree";

            SelectFirstProcedureName = "spSelectFirst" + TableName;
            SelectLastProcedureName = "spSelectLast" + TableName;
            SelectFirstProcedureNameFullTree = "spSelectFirst" + TableName + "FullTree";
            SelectLastProcedureNameFullTree = "spSelectLast" + TableName + "FullTree";

            UpdateProcedureName = "spUpdate" + EntityName;

            InsertProcedureName = "spInsert" + EntityName;

            SaveProcedureName = "spSave" + EntityName;

            
            /* Event Streams */
            
            var eventStreamEntity = entityType.Name;

            if (eventStreamEntity.StartsWith("I"))
            {
                eventStreamEntity = eventStreamEntity.Substring(1, eventStreamEntity.Length - 1);
            }

            if (eventStreamEntity.EndsWith("Base"))
            {
                eventStreamEntity = eventStreamEntity.Substring(0, eventStreamEntity.Length - 4);
            }
            
            if (eventStreamEntity.EndsWith("Event"))
            {
                eventStreamEntity = eventStreamEntity.Substring(0, eventStreamEntity.Length - 5);
            }

            EventStreamEntity = eventStreamEntity;

            if (entityType.GetCustomAttribute<OwnerNameAttribute>() is { } attribute)
            {
                EventStreamTableName = attribute.TableName+ "EventStream";
            }
            else
            {
                EventStreamTableName = EventStreamEntity.ToPlural() + "EventStream";
            }
            
            InsertEvent = "spInsert" + EventStreamEntity + "Event";

            ReadAllStreams = "spReadAll" + EventStreamEntity + "Streams";
            
            ReadStreamByStreamId = "spRead" + EventStreamEntity + "StreamByStreamId";
            
            ReadAllStreamsChunks = "spReadAll" + EventStreamEntity + "StreamsChunk";
            
            ReadStreamChunkByStreamId = "spRead" + EventStreamEntity + "StreamChunkByStreamId";

            /* filtering */
            
            PerformFilterIfNeededProcedureName = "spPerform" + TableName + "FilterIfNeeded";
            
            PerformFilterIfNeededProcedureNameFullTree = "spPerform" + TableName + "FilterIfNeededFullTree";
            
            ReadChunkProcedureName = "spRead" + TableName + "Chunk";
            
            ReadChunkProcedureNameFullTree = "spRead" + TableName + "ChunkFullTree";

            RangeProcedureName = "sp" + TableName + "Range";
            
            ExistingValuesProcedureName = "sp" + TableName + "ExistingValues";

            FilterResultsTableName = TableName + "FilterResults";
            
            SearchIndexTableName = TableName + "SearchIndex";

            RemoveExpiredFilterResultsProcedureName = "spRemoveExpired" + TableName + "FilterResults";
        }

        public string DeleteByIdProcedureName { get; }

        public string DeleteAllProcedureName { get; }

        public string SelectByIdProcedureName { get; }

        public string SelectByIdProcedureNameFullTree { get; }

        public string SelectAllProcedureName { get; }


        public string SelectFirstProcedureName { get; }

        public string SelectLastProcedureName { get; }

        public string SelectFirstProcedureNameFullTree { get; }

        public string SelectLastProcedureNameFullTree { get; }

        public string SelectAllProcedureNameFullTree { get; }


        public string UpdateProcedureName { get; }


        public string InsertProcedureName { get; }
        
        
        public string SaveProcedureName { get; }

        public string RemoveExpiredFilterResultsProcedureName { get; }
        
        public string PerformFilterIfNeededProcedureName { get; }
        public string PerformFilterIfNeededProcedureNameFullTree { get; }
        
        public string ReadChunkProcedureName { get; }
        
        public string ReadChunkProcedureNameFullTree { get; }
    }

    public class NameConvention<TEntity> : NameConvention
    {
        public NameConvention() : base(typeof(TEntity))
        {
        }

        public NameConvention(IDataOwnerNameProvider tableNameProvider) : base(typeof(TEntity), tableNameProvider)
        {
        }
    }
}