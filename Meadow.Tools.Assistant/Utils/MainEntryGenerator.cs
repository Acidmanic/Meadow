using System;
using System.Collections.Generic;
using System.Text;
using Acidmanic.Utilities.Results;

namespace Meadow.Tools.Assistant.Utils
{
    /// <summary>
    /// This class helps create small wrapping code around a code to shape a
    /// main-entry class's code
    /// </summary>
    public class MainEntryGenerator
    {
        public Result<string,string> GenerateMainEntry(List<string> usingNamespaces,
            string @namespace, string code)
        {
            var sbHeader = new StringBuilder();
            var footer = "";

            sbHeader.AppendLine("using System;");
            usingNamespaces.ForEach(p => sbHeader.AppendLine("using " + p + ";"));
            sbHeader.AppendLine();
            sbHeader.Append("namespace ").AppendLine(@namespace).AppendLine("{");
            footer = "}\n" + footer;

            var classname = "C" + Guid.NewGuid().ToString().Replace("-", "");
            sbHeader.Append("\tpublic class ").AppendLine(classname).AppendLine("\t{");
            footer = "\t}\n" + footer;

            sbHeader.AppendLine("\t\tpublic static void Main(string[] args)").AppendLine("\t\t{");
            footer = "\t\t}\n" + footer;

            sbHeader.AppendLine(code);
            sbHeader.AppendLine(footer);

            var result = new Result<string, string>(true, classname, sbHeader.ToString());

            return result;
        }
    }
}