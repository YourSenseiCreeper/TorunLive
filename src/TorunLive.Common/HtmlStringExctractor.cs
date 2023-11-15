﻿namespace TorunLive.Common
{
    public static class HtmlStringExctractor
    {
        private static readonly Dictionary<string, string> XmlEscapeReplacements = new()
        {
            { "<br>", "<br></br>" },
            { "&", "&amp;" }
        };

        public static string GetTextBetweenAndClean(string text, string start, string end)
        {
            return GetTextBetweenAndClean(text, start, end, XmlEscapeReplacements);
        }

        public static string GetTextBetweenAndClean(string text, string start, string end, Dictionary<string, string> elementsToReplace)
        {
            var startIndex = text.IndexOf(start);
            var endIndex = text.IndexOf(end);
            var substring = text[startIndex..(endIndex + end.Length)];
            foreach (var replacement in elementsToReplace)
            {
                substring = substring.Replace(replacement.Key, replacement.Value);
            }

            return substring;
        }
    }
}