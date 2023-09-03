using System;
using System.Collections.Generic;
using System.IO;

namespace Meadow.Scaffolding.Macros;

public class ScriptMacroExtractor
{
    public List<DetectedMacroPointer> ExtractMacros(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        List<DetectedMacroPointer> stickers = new();

        for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];

            var macroContents = GetMacroContent(line);

            foreach (var content in macroContents)
            {
                var sticker = new DetectedMacroPointer();

                if (ParseInto(content, sticker))
                {
                    sticker.FilePath = filePath;

                    sticker.StickerLineIndex = lineIndex;

                    stickers.Add(sticker);
                }
            }
        }

        return stickers;
    }

    private bool ParseInto(string content, DetectedMacroPointer pointer)
    {
        string chsep = char.ConvertFromUtf32(0);

        content = content.Replace(chsep, "");

        content = content.Replace("\\ ", chsep);

        var segments = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length > 0)
        {
            var name = segments[0].Replace(chsep, " ");

            if (!string.IsNullOrWhiteSpace(name))
            {
                pointer.Name = name;

                if (segments.Length > 1)
                {
                    var parameters = new List<string>();

                    for (int i = 1; i < segments.Length; i++)
                    {
                        var segment = segments[i];

                        if ("by-force".Equals(segment, StringComparison.OrdinalIgnoreCase))
                        {
                            pointer.ExternalToolReplacementMode = ExternalToolReplacementMode.ByForce;
                        }
                        else if ("always".Equals(segment, StringComparison.OrdinalIgnoreCase))
                        {
                            pointer.ExternalToolReplacementMode = ExternalToolReplacementMode.Always;
                        }
                        else
                        {
                            parameters.Add(segment);
                        }
                    }

                    pointer.Parameters = parameters.ToArray();
                }

                return true;
            }
        }

        return false;
    }

    private List<string> GetMacroContent(string line)
    {
        var macroContents = new List<string>();

        if (IsCommentLine(line))
        {
            var keepSearching = true;

            var lookUpIndex = 0;

            while (keepSearching)
            {
                keepSearching = false;

                var st = line.IndexOf("{{", lookUpIndex, StringComparison.Ordinal);

                if (st != -1)
                {
                    var nd = line.IndexOf("}}", st + 1, StringComparison.Ordinal);

                    if (nd != -1)
                    {
                        var content = line.Substring(st + 2, nd - st - 2);

                        macroContents.Add(content);

                        keepSearching = true;

                        lookUpIndex = nd + 2;
                    }
                }
            }
        }

        return macroContents;
    }

    private bool IsCommentLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        line = line.Trim();

        return line.StartsWith("#") || line.StartsWith("-- ");
    }
}