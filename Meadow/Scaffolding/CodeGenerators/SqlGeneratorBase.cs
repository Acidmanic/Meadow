using System;
using System.Linq;
using Meadow.DataTypeMapping;
using Meadow.Reflection;
using Meadow.Reflection.Conventions;
using Meadow.Reflection.ObjectTree;

namespace Meadow.Scaffolding.CodeGenerators
{
    public abstract class SqlGeneratorBase : ICodeGenerator
    {
        public Code Generate()
        {
            return Generate(false);
        }

        public Type Type { get; }

        protected AccessNode TreeRoot { get; }

        protected AccessNode RootOnlyNode { get; }

        protected AccessTreeInformation TreeInformation { get; }

        protected string EntityName { get; }

        public ITableNameProvider TableNameProvider { get; set; }

        protected string TableName { get; }

        protected IDbTypeNameMapper TypeNameMapper { get; }

        public abstract string SqlObjectName { get; }

        public SqlGeneratorBase(Type type)
        {
            Type = type;

            TreeRoot = new TypeAnalyzer().ToAccessNode(Type, true);

            RootOnlyNode = new TypeAnalyzer().ToAccessNode(Type, false);

            TreeInformation = new AccessTreeInformation(TreeRoot);

            TableNameProvider = new PluralTableNameProvider();

            TableName = GetTableName(Type);

            TypeNameMapper = new SqlDbTypeNameMapper();

            EntityName = Type.Name;
        }


        protected string GetTableName(Type type)
        {
            return TableNameProvider.GetTableName(type);
        }

        protected AccessNode GetIdField(AccessNode node)
        {
            return node.GetDirectLeaves().SingleOrDefault(leaf => leaf.IsUnique);
        }

        protected AccessNode GetIdField(Type type)
        {
            return GetIdField(new TypeAnalyzer() {TableNameProvider = TableNameProvider}.ToAccessNode(type, false));
        }

        public abstract Code Generate(bool alreadyExists);

        protected string CreateKeyWord(bool alreadyExists)
        {
            return alreadyExists ? "ALTER" : "CREATE";
        }

        protected void WalkThroughLeaves(bool fullTree, Action<AccessNode> leafAction)
        {
            var node = new TypeAnalyzer {TableNameProvider = TableNameProvider}.ToAccessNode(Type, fullTree);

            var info = new AccessTreeInformation(node);

            foreach (var leaf in info.OrderedLeaves)
            {
                leafAction(leaf);
            }
        }
    }
}