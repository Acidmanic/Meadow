using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree.StandardData;
using Meadow.Requests;
using Meadow.Sql;

namespace Meadow.SQLite
{
    public class SqLiteStorageAdapter : SqlDataStorageAdapterBase
    {
        // protected override void WriteAllToCommand(List<DataPoint> data, IDbCommand command)
        // {
        //
        //     var commandText = command.CommandText;
        //     
        //     var procedure = SqLiteInMemoryProcedures.Instance.GetProcedureOrNull(commandText);
        //
        //     if (procedure != null)
        //     {
        //         var injectedCode = InjectValuesIntoCode(procedure, data);
        //
        //         commandText = injectedCode;   
        //     }
        //     else
        //     {
        //         SqLiteProcedure p = SqLiteProcedure.Parse(commandText);
        //
        //         if(p!=null)
        //         {
        //             SqLiteInMemoryProcedures.Instance.AddProcedure(p);
        //
        //             commandText = "";
        //         }
        //     }
        //
        //     command.CommandText = commandText;
        //     
        // }

        

        protected override void WriteIntoCommand(DataPoint dataPoint, IDbCommand command)
        {
            // We dont do that here!
            //throw new NotImplementedException();
        }
    }
}