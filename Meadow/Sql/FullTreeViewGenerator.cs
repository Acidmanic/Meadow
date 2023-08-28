using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Acidmanic.Utilities.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using Meadow.Extensions;
using Meadow.RelationalStandardMapping;

namespace Meadow.Sql
{
    public class FullTreeViewGenerator
    {
        public class SelectField
        {
            public FieldKey Key { get; set; }

            public string Source { get; set; }
        }

        public class View
        {
            public View(Dictionary<string, string> translatedFields)
            {
                TranslatedFields = translatedFields;
            }

            private Dictionary<string, string> TranslatedFields { get; }
            public string ViewName { get; set; }

            public Type Type { get; set; }

            public List<SelectField> Fields { get; set; } = new List<SelectField>();

            public List<string>  Joins { get; set; } = new List<string>();

            public override string ToString()
            {
                var result = $"CREATE VIEW {ViewName}FullTreeView AS \n  SELECT\n";

                var sep = "\t";

                foreach (var field in Fields)
                {
                    var fieldSelect = TranslatedFields[field.Key.ToString()];

                    result += sep + $"{field.Source}.{field.Key.TerminalSegment().Name} '{fieldSelect}'";

                    sep = ",\n\t";
                }

                result += $"\nFROM {ViewName}\n";

                sep = "\t";
                foreach (var j in Joins)
                {
                    result += sep + Joins;
                    sep = "\t\n";
                }
                
                return result;
            }
        }


        public IDataOwnerNameProvider DataOwnerNameProvider { get; set; } = new PluralDataOwnerNameProvider();

        public char Delimiter { get; set; } = '_';

        public string GetViewDefinition<T>()
        {
            var type = typeof(T);

            var viewName = DataOwnerNameProvider.GetNameForOwnerType(type) + "FullTree";


            var evaluator = new ObjectEvaluator(type);

            var map = new ConditionalRelationalToStandardMapper()
                    {Separator = Delimiter, DataOwnerNameProvider = DataOwnerNameProvider}
                .MapAddressesByIdentifier(type, true);

            var selectees = new List<string>();

            var sep = "";

            foreach (var item in map)
            {
                var key = item.Value;

                var node = evaluator.Map.NodeByKey(key);

                var ownerType = node.Parent.Type;

                var typePath = GetTypePath(node);

                var sourcename = GetPathOwnerNames(typePath) + "." + key.TerminalSegment().Name;

                var fieldName = item.Key;

                selectees.Add(sep + sourcename + " '" + fieldName + "'");

                sep = ",\n\t";
            }

            selectees.Sort();

            string view = $"CREATE VIEW {viewName} AS\nSELECT\n";

            selectees.ForEach(s => view += s);


            return view;
        }

        private string GetPathOwnerNames(Type[] typePath)
        {
            var path = "";

            var sep = "";

            for (int i = 0; i < typePath.Length; i++)
            {
                path += sep + DataOwnerNameProvider.GetNameForOwnerType(typePath[i]);

                sep = ".";
            }

            return path;
        }

        private Type[] GetTypePath(AccessNode node)
        {
            var pathTypes = new List<Type>();

            var parent = node.Parent;

            while (parent != null)
            {
                if (!parent.IsCollection)
                {
                    pathTypes.Add(parent.Type);
                }

                parent = parent.Parent;
            }

            pathTypes.Reverse();

            return pathTypes.ToArray();
        }


        public View GetViewStructure<T>()
        {
            var type = typeof(T);

            var evaluator = new ObjectEvaluator(type);

            var node = evaluator.RootNode;

            var map = new ConditionalRelationalToStandardMapper()
            {
                Separator = Delimiter,
                DataOwnerNameProvider = DataOwnerNameProvider
            }.MapAddressesByIdentifier(type, true);

            var translated = new Dictionary<string, string>();

            foreach (var item in map)
            {
                translated.Add(item.Value.ToString(), item.Key);
            }

            return GetViewDefinition(node, evaluator, translated);
        }


        private View GetViewDefinition(AccessNode node, ObjectEvaluator evaluator,
            Dictionary<string, string> translated)
        {
            var result = new View(translated)
            {
                ViewName = DataOwnerNameProvider.GetNameForOwnerType(node.Type),
                Fields = new List<SelectField>(),
                Type = node.Type,
            };

            var children = node.GetChildren();

            foreach (var child in children)
            {
                var key = evaluator.Map.FieldKeyByNode(child);

                if (child.IsLeaf)
                {
                    var field = new SelectField
                    {
                        Key = key,
                        Source = result.ViewName
                    };
                    result.Fields.Add(field);
                }
                else
                {
                    var inner = child;
                    var linkForward = true;

                    if (child.IsCollection)
                    {
                        inner = child.GetChildren()[0];

                        linkForward = false;
                    }

                    var childView = GetViewDefinition(inner, evaluator, translated);

                    result = Join(result, childView, linkForward, translated);
                }
            }


            return result;
        }

        private View Join(View main, View childView, in bool linkForward, Dictionary<string, string> translated)
        {
            var joined = new View(translated)
            {
                ViewName = main.ViewName,
                Fields = new List<SelectField>(main.Fields),
                Type = main.Type
            };
            var joinName = main.ViewName + Delimiter + childView.ViewName;

            var link = linkForward?GetJoinLink(main, childView):GetJoinLink(childView, main);

            if (link != null)
            {
                joined.Joins.Add($" JOIN {childView.ViewName} ON " + link);
            }
            childView.Fields.ForEach(childField =>
            {
                childField.Source = main.ViewName + Delimiter + childField.Source;
                joined.Fields.Add(childField);
            });

            return joined;
        }

        private string GetJoinLink(View main, View childView)
        {

            foreach (var field in childView.Fields)
            {
                var name = childView.ViewName + field.Key.TerminalSegment().Name;

                var foundSource = main.Fields
                    .FirstOrDefault(f => f.Key.TerminalSegment().Name == name);
                
                if (foundSource!=null)
                {
                    return main.ViewName + "." + foundSource.Key.TerminalSegment().Name + " = "
                           + childView.ViewName + "." + name;
                }
            }

            return null;
        }

        // private View GetViewDefinition(AccessNode node,ObjectEvaluator evaluator)
        // {
        //     var properties = type.GetProperties();
        //
        //     var result = new View();
        //     
        //     result.Fields = new List<string>();
        //     
        //     foreach (var property in properties)
        //     {
        //         var t = property.PropertyType;
        //
        //         if (TypeCheck.IsEffectivelyPrimitive(t))
        //         {
        //             result.Fields.Add(property.Name);
        //         }
        //     }
        //
        //     result.Source = DataOwnerNameProvider.GetNameForOwnerType(type);
        //
        //     return result;
        // }
    }
}