namespace TorunLive.SIPTimetableScanner
{
    public static class Common
    {
        public static readonly Dictionary<string, string> XmlEscapeReplacements = new()
        {
            { "<br>", "<br></br>" },
            { "&", "&amp;" }
        };

        public static string GetTextBetweenAndClean(string text, string start, string end, Dictionary<string, string> elementsToReplace)
        {
            var startIndex = text.IndexOf(start);
            var endIndex = text.IndexOf(end);
            var substring = text.Substring(startIndex, endIndex + end.Length - startIndex);
            foreach (var replacement in elementsToReplace)
            {
                substring = substring.Replace(replacement.Key, replacement.Value);
            }

            return substring;
        }
    }
}
