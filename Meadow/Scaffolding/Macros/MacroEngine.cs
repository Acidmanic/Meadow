using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Meadow.Scaffolding.Macros;

public class MacroEngine
{
    private readonly Assembly[] _assemblies;


    public MacroEngine(params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            _assemblies = new[] { Assembly.GetCallingAssembly() };
        }
        else
        {
            _assemblies = assemblies;
        }
    }

    public void ExecuteMacrosFor(string directory)
    {
        var files = new DirectoryInfo(directory).GetFiles("*.sql");

        ExecuteMacrosFor(files);
    }
    
    
    public void ExecuteMacrosFor(FileInfo[] files)
    {
        foreach (var file in files)
        {
            if (IsScriptFile(file))
            {
                var filePath = file.FullName;

                var ex = new ScriptMacroExtractor();

                var stickers = ex.ExtractMacros(filePath);

                Execute(stickers, file);
            }
        }
    }

    private void Execute(List<DetectedMacroPointer> stickers, FileInfo file)
    {
        var stickersByLine = GroupByLine(stickers);

        var factory = new MacroFactory();

        foreach (var assembly in _assemblies)
        {
            factory.ScanAssembly(assembly);
        }
        
        var updateLines = new Dictionary<long, string>();

        foreach (var stickerGroup in stickersByLine)
        {
            string contentHeader = "-- ";

            string content = "";

            var sep = "";

            foreach (var sticker in stickerGroup.Value)
            {
                var macro = factory.Make(sticker.Name);

                content += macro.GenerateCode(sticker.Parameters) + "";

                contentHeader += sep + sticker.Name +
                                 (sticker.Parameters.Length > 0 ? " " : "")
                                 + string.Join(" ", sticker.Parameters);

                sep = " - ";
            }

            content = contentHeader + "\n" + content;

            updateLines.Add(stickerGroup.Key, content);
        }

        UpdateFileLines(file, updateLines);
    }

    private void UpdateFileLines(FileInfo file, Dictionary<long, string> updateLines)
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

        if (file.Exists)
        {
            file.Delete();
        }

        File.WriteAllText(file.FullName, content.ToString());
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

    private bool IsScriptFile(FileInfo file)
    {
        var name = file.Name;

        if (name.Length < 10)
        {
            return false;
        }

        if (name[4] != '-')
        {
            return false;
        }

        if (!name.ToLower().EndsWith(".sql"))
        {
            return false;
        }

        var numberString = name[0..4];

        return int.TryParse(numberString, out _);
    }
}