using System;
using System.Collections.Generic;
using System.Text;
using Meadow.Test.Functional.Search.Contracts;

namespace Meadow.Test.Functional.Search.Services
{
    public class EnglishTransliterationsService : ITransliterationService
    {
        private readonly Dictionary<string, string> _replaces = new Dictionary<string, string>()
        {
            { "ou", "u" },
            { "oo", "o" },
            { "ee", "i" },
            { "y", "i" },
            { "sch", "$" },
            { "sh", "$" },
            { "ce", "se" },
            { "ci", "si" },
            { "ck", "k" }
        };
    
        public string Transliterate(string text)
        {
            var transliterated = FilterLetterOrDigit(text);
    
            foreach (var replace in _replaces)
            {
                transliterated = transliterated.Replace(replace.Key, replace.Value);
            }
    
            transliterated = CompressLossy(transliterated);
    
            return transliterated;
        }
    
    
        private string CompressLossy(string text)
        {
            var segments = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    
            var chosen = new List<string>();
    
            foreach (var segment in segments)
            {
                if (!chosen.Contains(segment))
                {
                    chosen.Add(segment);
                }
            }
    
            var compressed = string.Join(' ', chosen);
    
            return compressed;
        }
    
        private string FilterLetterOrDigit(string text)
        {
            text ??= "";
    
            var sb = new StringBuilder();
    
            var chars = text.ToCharArray();
    
            foreach (var c in chars)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(char.ToLower(c));
                }
    
                if (char.IsWhiteSpace(c))
                {
                    sb.Append(' ');
                }
            }
    
            return sb.ToString();
        }
    }
}