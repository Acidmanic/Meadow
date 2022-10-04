// using System.Collections.Generic;
// using Meadow.Models;
// using Meadow.Requests;
//
// namespace Meadow.Configuration.ConfigurationRequests
// {
//     public class EnumerateProceduresRequest:ConfigurationFunctionRequest<NameResult>
//     {
//         protected override string GetRequestText()
//         {
//             return "SELECT name 'Name' FROM sys.objects WHERE type_desc = 'SQL_STORED_PROCEDURE';";
//         }
//     }
// }