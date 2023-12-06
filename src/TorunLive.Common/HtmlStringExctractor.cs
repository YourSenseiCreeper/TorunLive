namespace TorunLive.Common
{
    public static class HtmlStringExctractor
    {
        private static readonly Dictionary<string, string> XmlEscapeReplacements = new()
        {
            { "<br>", "<br></br>" },
            { "&", "&amp;" }
        };

        public static string GetTextBetweenAndClean(string text, string start, string end, bool includeEnding = true)
        {
            return GetTextBetweenAndClean(text, start, end, includeEnding, XmlEscapeReplacements);
        }

        public static string GetTextBetweenAndClean(string text, string start, string end, bool includeEnding, Dictionary<string, string> elementsToReplace)
        {
            var startIndex = text.IndexOf(start);
            var endIndex = text.IndexOf(end);
            var endWithEndingIndex = includeEnding ? endIndex + end.Length : endIndex;
            var substring = text[startIndex..endWithEndingIndex];
            foreach (var replacement in elementsToReplace)
            {
                substring = substring.Replace(replacement.Key, replacement.Value);
            }

            return substring;
        }
    }
}
