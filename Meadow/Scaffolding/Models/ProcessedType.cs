using System;
using System.Collections.Generic;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Contracts;
using Meadow.Scaffolding.Attributes;

namespace Meadow.Scaffolding.Models
{
    /// <summary>
    /// This class would contain common information that might be needed for generating scripts. 
    /// </summary>
    public class ProcessedType
    {
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        public List<Parameter> NoneIdParameters { get; set; } = new List<Parameter>();

        public List<Parameter> NoneIdUniqueParameters { get; set; } = new List<Parameter>();

        public Parameter IdParameter { get; set; }


        public List<Parameter> ParametersFullTree { get; set; } = new List<Parameter>();

        public List<Parameter> NoneIdParametersFullTree { get; set; } = new List<Parameter>();

        public List<Parameter> NoneIdUniqueParametersFullTree { get; set; } = new List<Parameter>();

        public Parameter IdParameterFullTree { get; set; }

        public NameConvention NameConvention { get; set; }

        public AccessNode IdField { get; set; }

        public bool HasId { get; set; }


        public bool IsStreamEvent { get; set; }

        public EventStreamInfo EventStream { get; set; }

        public string EventIdTypeName { get; set; }

        public string StreamIdTypeName { get; set; }

        public string EventStreamTypeNameDatabaseType { get; set; }

        public string EventStreamSerializedValueDatabaseType { get; set; }

        public bool IsEventIdAutogenerated { get; set; }
    }
}