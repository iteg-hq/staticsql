using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace StaticSQL
{
    public delegate string FormatterDelegate(string s);
    
    public static class Quoting
    {
        private readonly static Regex identifier = new Regex(Properties.Resources.IdentifierRegex);

        private readonly static ISet<string> Reserved = new HashSet<string>(Properties.Resources.Reserved.Split(',').Select(r => r.ToLower().Trim()));
        private static bool IsReserved(string s) => Reserved.Contains(s.ToLower());

        private static bool NeedsQuotes(string s) => IsReserved(s) || !identifier.IsMatch(s);

        public static string Never(string s) => s;

        public static string Always(string s) => string.Format("[{0}]", s);

        public static string AsNeeded(string s) => NeedsQuotes(s) ? Always(s) : s;
    }

    public static class Formatting
    {
        private readonly static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public static FormatterDelegate Combine(FormatterDelegate formatter1, FormatterDelegate formatter2) => s => formatter2(formatter1(s));

        public static string NoFormatting(string s) => s;

        public static string NoSpaceCase(string s) => s.Replace(" ", "");

        public static string UnderscoreCase(string s) => s.Replace(" ", "_");

        public static string TitleCase(string s) => textInfo.ToTitleCase(s);

        public static string UpperCase(string s) => textInfo.ToUpper(s);

        public static string LowerCase(string s) => textInfo.ToLower(s);

        public static string FirstLetterSmall(string s) => textInfo.ToLower(s.Substring(0, 1)) + s.Substring(1);

        public static string PascalCase(string s) => NoSpaceCase(TitleCase(s));

        public static string CamelCase(string s) => FirstLetterSmall(PascalCase(s));

        public static string SnakeCase(string s) => UnderscoreCase(LowerCase(s));

        public static string ScreamingSnakeCase(string s) => UnderscoreCase(UpperCase(s));
    }
}
