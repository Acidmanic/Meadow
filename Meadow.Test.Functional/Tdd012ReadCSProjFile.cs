using System;
using System.IO;
using System.Net.Http.Headers;
using System.Xml;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd012ReadCsProjFile : MeadowFunctionalTest
    {
        public override void Main()
        {
            var path = "../../../Meadow.Test.Functional.csproj";

            var ns = GetRootNamespace(path);

            Console.WriteLine("Root Namespace: " + ns);
        }

        private void PrintXml(XmlNode root, string indent)
        {
            Console.WriteLine(indent + root.Value);

            foreach (XmlNode child in root.ChildNodes)
            {
                PrintXml(child, indent + "|    ");
            }
        }

        private string GetRootNamespace(XmlNode root)
        {
            if (root.Name == "RootNamespace")
            {
                return root.InnerText;
            }

            foreach (XmlNode child in root.ChildNodes)
            {
                var rootNamespace = GetRootNamespace(child);

                if (rootNamespace != null)
                {
                    return rootNamespace;
                }
            }

            return null;
        }

        private string GetRootNamespace(string projectFile)
        {
            if (!string.IsNullOrEmpty(projectFile) && File.Exists(projectFile))
            {
                XmlDocument doc = new XmlDocument();

                var content = File.ReadAllText(projectFile);

                doc.LoadXml(content);

                XmlNode root = doc.FirstChild;

                var projRootNamespace = GetRootNamespace(root);

                if (projRootNamespace != null)
                {
                    return projRootNamespace;
                }

                var file = new FileInfo(projectFile);

                var name = file.Name;

                if (!string.IsNullOrEmpty(file.Extension))
                {
                    name = name.Substring(0, name.Length - file.Extension.Length);
                }

                return name;
            }

            return "";
        }
    }
}