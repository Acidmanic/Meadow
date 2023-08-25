using System.Collections.Generic;
using System.ComponentModel;

namespace Meadow.Scaffolding.Contracts;

public interface ICommentSplitter
{
    public class TextPart
    {
        public bool IsComment { get; set; }
        
        public string Text { get; set; }
    }

    List<TextPart> Split(string value);
}