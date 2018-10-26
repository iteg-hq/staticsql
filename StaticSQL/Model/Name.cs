using System.Linq;
using System.Globalization;
using System;
using System.Collections.Generic;

namespace StaticSQL
{
    public delegate string NameFormatter(Name name);

    public class Name
    {
        static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public static string UndefinedValue = "<undefined>";

        public static string NameSeparator = " ";

        // Default formatter, used for casting to string
        public static NameFormatter Formatter = name => name.QuoteIfNeeded();

        // Reserved SQL keywords, used to control quoting.
        static ISet<string> reserved = new HashSet<string>(Properties.Resources.Reserved.Split(',').Select(r => r.ToLower().Trim()));

        public readonly string RawValue;

        public bool IsReserved { get { return reserved.Contains(RawValue.ToLower()); } }

        public bool NeedsQuotes { get { return IsReserved || RawValue.Contains(' '); } }

        public string Quote() => string.Format("[{0}]", RawValue);

        public string QuoteIfNeeded() => NeedsQuotes ? string.Format("[{0}]", RawValue) : RawValue;

        public Name ToPascalCase() => new Name(textInfo.ToTitleCase(RawValue).Replace(" ", ""));

        public Name NoSpaces() => new Name(RawValue.Replace(" ", ""));

        public static Name operator +(string str, Name name) => new Name(str + NameSeparator + name.RawValue);

        public static Name operator +(Name name, string str) => new Name(name.RawValue + NameSeparator + str);

        public static Name operator +(Name name1, Name name2) => new Name(name1.RawValue + NameSeparator + name2.RawValue);

        // Name <-> string
        public Name(string rawName) => RawValue = rawName;

        public Name() : this(Name.UndefinedValue) { }

        public override string ToString() => Formatter(this);

        public static implicit operator Name(string rawName) => new Name(rawName);

        public static implicit operator string(Name name) => name.ToString();

    }
}
