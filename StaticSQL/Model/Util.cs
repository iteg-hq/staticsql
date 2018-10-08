using System.Linq;
using System.Globalization;

namespace StaticSQL
{
    public static class Util
    {
        static string[] reserved = {
            "policy"
        };

        static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public static bool NeedsQuotes(string word)
        {
            return reserved.Contains(word.ToLower()) ||word.Contains(' ');
        }

        public static string Quote(string word)
        {
            return string.Format("[{0}]", word);
        }

        public static string QuoteIfNeeded(string word)
        {
            return Util.NeedsQuotes(word) ? string.Format("[{0}]", word) : word;
        }

        public static string ToPascalCase(string str)
        {
            return textInfo.ToTitleCase(str).Replace(" ", "");
        }
    }
}