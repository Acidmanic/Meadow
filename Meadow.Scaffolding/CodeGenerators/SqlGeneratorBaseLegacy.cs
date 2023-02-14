using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Contracts;
using Meadow.DataTypeMapping;

namespace Meadow.Scaffolding.CodeGenerators
{
    public abstract class SqlGeneratorBaseLegacy : ICodeGenerator
    {
        public Code Generate()
        {
            return Generate(SqlScriptActions.Create);
        }

        public abstract DbObjectTypes ObjectType { get; }
        public Type Type { get; }

        protected AccessNode TreeRoot { get; }

        protected AccessNode RootOnlyNode { get; }

        protected AccessTreeInformation TreeInformation { get; }

        protected IDbTypeNameMapper TypeNameMapper { get; private set; }

        public abstract string SqlObjectName { get; }

        protected NameConvention NameConvention { get; }

        protected IEnumerable<AccessNode> UniqueNodes { get; }

        protected AccessNode IdField => HasIdField ? UniqueNodes.FirstOrDefault() : null;

        protected bool HasIdField => UniqueNodes.Any();

        protected void UseDbTypeMapper(IDbTypeNameMapper mapper)
        {
            this.TypeNameMapper = mapper;
        }
        
        public SqlGeneratorBaseLegacy(Type type)
        {
            Type = type;

            TreeRoot = ObjectStructure.CreateStructure(Type, true);

            RootOnlyNode = ObjectStructure.CreateStructure(Type, false);

            TreeInformation = new AccessTreeInformation(TreeRoot);

            TypeNameMapper = new SqlDbTypeNameMapper();

            NameConvention = new NameConvention(type);

            UniqueNodes = RootOnlyNode.GetDirectLeaves().Where(n => n.IsUnique);
        }


        public abstract Code Generate(SqlScriptActions action);

        protected string CreateKeyWord(bool alreadyExists)
        {
            return alreadyExists ? "ALTER" : "CREATE";
        }

        
    }
}