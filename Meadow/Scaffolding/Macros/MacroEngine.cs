using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Meadow.Configuration;

namespace Meadow.Scaffolding.Macros;

public class MacroEngine
{
    private readonly Assembly[] _assemblies;
    private readonly MeadowConfiguration _configuration;
    
    
    public MacroEngine(MeadowConfiguration configuration, params Assembly[] assemblies)
    {
        _configuration = configuration;
        
        if (assemblies.Length == 0)
        {
            _assemblies = new[] { Assembly.GetCallingAssembly() };
        }
        else
        {
            _assemblies = assemblies;
        }
    }

    /// <summary>
    /// Enumerates '.sql' files inside the directory (matching with filter if not null), then applies detected macros on
    /// each file. This method does update the file on disk. 
    /// </summary>
    /// <param name="directory">Where .sql files are expected to be.</param>
    /// <param name="filter">If returns 'True', the file would be processed, otherwise the file would be ignored. </param>
    public void ExecuteMacrosFor(string directory, Func<FileInfo, bool> filter = null)
    {
        var files = new DirectoryInfo(directory).GetFiles("*.sql");

        filter ??= (f => true);

        files = files.Where(filter).ToArray();

        ExecuteMacrosFor(files, (det, content) => true);
    }

    /// <summary>
    /// Enumerates over given files to apply macros. For each file calls macroEvaluation, so that user can make use of
    /// updated file content and choose to update the file content or not.
    /// </summary>
    /// <param name="files">List of script files to go through</param>
    /// <param name="macroEvaluation">
    /// This expression, provides a bool value to indicate if any macros where detected for each file. and a string
    /// value which is the updated content of the file. If this expression, returns 'True' the updated content would be
    /// saved into the file.
    /// </param>
    public void ExecuteMacrosFor(FileInfo[] files, Func<bool, string, bool> macroEvaluation)
    {
        foreach (var file in files)
        {
           ExecuteMacrosFor(file,macroEvaluation);
        }
    }

    /// <summary>
    /// For the given file, calls macroEvaluation, so that user can make use of
    /// updated file content and choose to update the file content or not.
    /// </summary>
    /// <param name="file">The file to be processed</param>
    /// <param name="macroEvaluation">
    /// This expression, provides a bool value to indicate if any macros where detected for each file. and a string
    /// value which is the updated content of the file. If this expression, returns 'True' the updated content would be
    /// saved into the file.
    /// </param>
    public void ExecuteMacrosFor(FileInfo file, Func<bool, string, bool> macroEvaluation)
    {
        var filePath = file.FullName;

        var ex = new ScriptMacroExtractor();

        var pointers = ex.ExtractMacros(filePath);

        var updates = EvaluateMacros(pointers);

        var anyMacros = pointers.Count > 0;

        var content = GetMacroAppliedContent(file, updates);

        if (macroEvaluation(anyMacros, content))
        {
            EnsureContentWritten(file, content);
        }
    }


    private Dictionary<long, string> EvaluateMacros(List<DetectedMacroPointer> pointers)
    {
        var pointerByLine = GroupByLine(pointers);

        var factory = new MacroFactory();

       factory.ScanAssemblies(_assemblies);

        var updateLines = new Dictionary<long, string>();

        foreach (var stickerGroup in pointerByLine)
        {
            string contentHeader = "-- ";

            string content = "";

            var sep = "";

            foreach (var sticker in stickerGroup.Value)
            {
                var macro = factory.Make(sticker.Name);

                macro.LoadedAssemblies = new List<Assembly>(_assemblies);

                macro.Configuration = _configuration;
                
                content += macro.GenerateCode(sticker.Parameters) + "";

                contentHeader += sep + sticker.Name +
                                 (sticker.Parameters.Length > 0 ? " " : "")
                                 + string.Join(" ", sticker.Parameters);

                sep = " - ";
            }

            content = contentHeader + "\n" + content;

            updateLines.Add(stickerGroup.Key, content);
        }

        return updateLines;
    }


    private string GetMacroAppliedContent(FileInfo file, Dictionary<long, string> updateLines)
    {
        var lines = File.ReadAllLines(file.FullName);

        var content = new StringBuilder();

        for (long i = 0; i < lines.Length; i++)
        {
            if (updateLines.ContainsKey(i))
            {
                content.AppendLine(updateLines[i]);
            }
            else
            {
                content.AppendLine(lines[i]);
            }
        }

        return content.ToString();
    }

    private void EnsureContentWritten(FileInfo file, string content)
    {
        if (file.Exists)
        {
            file.Delete();
        }

        File.WriteAllText(file.FullName, content);
    }

    private Dictionary<long, List<DetectedMacroPointer>> GroupByLine(List<DetectedMacroPointer> stickers)
    {
        var stickersByLine = new Dictionary<long, List<DetectedMacroPointer>>();

        foreach (var sticker in stickers)
        {
            if (!stickersByLine.ContainsKey(sticker.StickerLineIndex))
            {
                stickersByLine.Add(sticker.StickerLineIndex, new List<DetectedMacroPointer>());
            }

            stickersByLine[sticker.StickerLineIndex].Add(sticker);
        }

        return stickersByLine;
    }
}