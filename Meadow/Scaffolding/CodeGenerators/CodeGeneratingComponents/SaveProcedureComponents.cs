using System.Collections.Generic;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.CodeGenerators.CodeGeneratingComponents;

public record SaveProcedureComponents(string ProcedureName, List<Parameter> WhereEqualities, List<Parameter> InsertUpdateParameters);