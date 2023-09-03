namespace Meadow.Scaffolding.Macros;

public class DetectedMacroPointer
{
    public string Name { get; set; }

    public string[] Parameters { get; set; } = { };
    
    public string FilePath { get; set; }
    
    public long StickerLineIndex { get; set; }

    public ExternalToolReplacementMode ExternalToolReplacementMode { get; set; } = ExternalToolReplacementMode.Regular;
}