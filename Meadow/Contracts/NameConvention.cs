using System;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.Contracts
{
    public class NameConvention
    {
        public string EntityName { get; private set; }

        public string TableName { get; private set; }

        public IDataOwnerNameProvider TableNameProvider { get; }
        public Type EntityType { get; private set; }

        public NameConvention(Type entityType) : this(entityType, new PluralDataOwnerNameProvider())
        {
        }

        public NameConvention(Type entityType, IDataOwnerNameProvider tableNameProvider)
        {
            EntityType = entityType;

            EntityName = EntityType.Name;

            TableNameProvider = tableNameProvider;

            TableName = TableNameProvider.GetNameForOwnerType(EntityType);


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