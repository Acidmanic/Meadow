using System;
using System.Text.RegularExpressions;

namespace Meadow.SQLite.ProcedureProcessing
{
    public static class StringExtensions
    {
        public static string SubString(this string main, int startIndex, string upToTarget, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(main))
            {
                return null;
            }

            var strRegex = ignoreCase ? new Regex(upToTarget, RegexOptions.IgnoreCase) : new Regex(upToTarget);

            var strMatch = strRegex.Match(main, startIndex);

            if (strMatch.Captures.Count == 0)
            {
                return null;
            }

            int targetStart = strMatch.Index;

            if (targetStart < startIndex)
            {
                return null;
            }

            return main.Substring(startIndex, targetStart - startIndex);
        }

        public static string SubStringBetween(this string main, string starter, string finisher,
            bool ignoreCase = false)
        {
            var strRegex = ignoreCase ? new Regex(starter, RegexOptions.IgnoreCase) : new Regex(starter);

            var strMatch = strRegex.Match(main, 0);

            if (strMatch.Captures.Count == 0)
            {
                return null;
            }

            var startIndex = strMatch.Index + strMatch.Length;

            return main.SubString(startIndex, finisher, ignoreCase);
        }


        public static string SubStringAfterTag(this string main, string starter, bool ignoreCase = false)
        {
            var strRegex = ignoreCase ? new Regex(starter, RegexOptions.IgnoreCase) : new Regex(starter);

            var strMatch = strRegex.Match(main, 0);

            if (strMatch.Captures.Count == 0)
            {
                return null;
            }

            int startIndex = strMatch.Index + strMatch.Length;

            return main.Substring(startIndex, main.Length - startIndex);
        }

        public static string SafeTrim(this string value)
        {
            return value?.Trim();
        }


        public static string[] SplitByWhiteSpaces(this string value)
        {
            return SplitByWhiteSpaces(value, StringSplitOptions.None);
        }

        public static string[] SplitByWhiteSpaces(this string value, StringSplitOptions options)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new string[] { };
            }

            return value.Split(new char[] {' ', '\t', '\n', '\r'}, options);
        }
    }
}