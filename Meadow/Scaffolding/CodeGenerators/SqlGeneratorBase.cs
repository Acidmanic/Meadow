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
            return Generate(SqlScriptActions.Create);
        }

        public abstract DbObjectTypes ObjectType { get; }
        public Type Type { get; }

        protected AccessNode TreeRoot { get; }

        protected AccessNode RootOnlyNode { get; }

        protected AccessTreeInformation TreeInformation { get; }

        protected IDbTypeNameMapper TypeNameMapper { get; }

        public abstract string SqlObjectName { get; }
        
        protected  NameConvention NameConvention { get; }

        public SqlGeneratorBase(Type type)
        {
            Type = type;

            TreeRoot = new TypeAnalyzer().ToAccessNode(Type, true);

            RootOnlyNode = new TypeAnalyzer().ToAccessNode(Type, false);

            TreeInformation = new AccessTreeInformation(TreeRoot);
            
            TypeNameMapper = new SqlDbTypeNameMapper();

            NameConvention = new NameConvention(type);
        }
        
        protected AccessNode GetIdField(AccessNode node)
        {
            return node.GetDirectLeaves().SingleOrDefault(leaf => leaf.IsUnique);
        }

        protected AccessNode GetIdField(Type type)
        {
            return GetIdField(new TypeAnalyzer() {TableNameProvider = NameConvention.TableNameProvider}.ToAccessNode(type, false));
        }

        public abstract Code Generate(SqlScriptActions action);

        protected string CreateKeyWord(bool alreadyExists)
        {
            return alreadyExists ? "ALTER" : "CREATE";
        }

        protected void WalkThroughLeaves(bool fullTree, Action<AccessNode> leafAction)
        {
            var node = new TypeAnalyzer {TableNameProvider = NameConvention.TableNameProvider}.ToAccessNode(Type, fullTree);

            var info = new AccessTreeInformation(node);

            foreach (var leaf in info.OrderedLeaves)
            {
                leafAction(leaf);
            }
        }
    }
}