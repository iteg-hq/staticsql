using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace StaticSQL
{
    public delegate string FormatterDelegate(string s);

    public interface IFormatter
    {
        string Format(string s);
    }

    public class Formatter : IFormatter
    {
        private readonly FormatterDelegate _delegate;

        public Formatter(FormatterDelegate _delegate)
        {
            this._delegate = _delegate;
        }

        public string Format(string s)
        {
            return _delegate(s);
        }
    }

    public class CombinedFormatter : IFormatter
    {
        private readonly IFormatter formatter1;
        private readonly IFormatter formatter2;

        public CombinedFormatter(IFormatter formatter1, IFormatter formatter2)
        {
            this.formatter1 = formatter1;
            this.formatter2 = formatter2;
        }

        public string Format(string s)
        {
            return formatter2.Format(formatter1.Format(s));
        }
    }


    public static class FormatterFactory
    {

        private readonly static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public static IFormatter NoSpaceCase() => new Formatter(s => s.Replace(" ", ""));

        public static IFormatter TitleCase() => new Formatter(s => textInfo.ToTitleCase(s));

        public static IFormatter UpperCase() => new Formatter(s => textInfo.ToUpper(s));

        public static IFormatter LowerCase() => new Formatter(s => textInfo.ToLower(s));

        public static IFormatter FirstLetterSmall() => new Formatter(s => textInfo.ToLower(s.Substring(0, 1)) + s.Substring(1));

        public static IFormatter PascalCase() => new CombinedFormatter(TitleCase(), NoSpaceCase());

        public static IFormatter CamelCase() => new CombinedFormatter(PascalCase(), FirstLetterSmall());

        public static IFormatter Underscored() => new Formatter(s => s.Replace(" ", "_"));

        public static IFormatter SnakeCase() => new CombinedFormatter(LowerCase(), Underscored());

        public static IFormatter ScreamingSnakeCase() => new CombinedFormatter(UpperCase(), Underscored());


        public static IFormatter PascalCaseQuoteIfNeeded() => new CombinedFormatter(PascalCase(), QuoteIfNeeded());

        private readonly static Regex identifier = new Regex(Properties.Resources.IdentifierRegex);

        private readonly static ISet<string> Reserved = new HashSet<string>(Properties.Resources.Reserved.Split(',').Select(r => r.ToLower().Trim()));

        /// <summary>
        /// Check if a word is reserved in T-SQL. Reserved words include
        /// those that would be highlighted by Visual Studio.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>Returns true iff s is reserved.</returns>
        public static bool IsReserved(string s) => Reserved.Contains(s.ToLower());

        /// <summary>
        /// Check if a name needs quotes when rendered as an identifier in a template, for instance if it has spaces,
        /// contains special characters or is reserved.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>Returns true if the name needs quotes</returns>
        public static bool NeedsQuotes(string s) => IsReserved(s) || !identifier.IsMatch(s);

        /// <summary>
        /// Returns the string, quoted.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IFormatter AlwaysQuote() => new Formatter(s => string.Format("[{0}]", s));

        /// IF hte string needs quotes, returns the string, quoted.
        public static IFormatter QuoteIfNeeded() => new Formatter(s => NeedsQuotes(s) ? string.Format("[{0}]", s) : s);
    }
}
