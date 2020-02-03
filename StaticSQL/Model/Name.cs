using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace StaticSQL
{
    public class Name
    {
        private readonly string value;

        private readonly IFormatter formatter;

        /// <summary>
        /// Construct a name with a given formatter
        /// </summary>
        /// <param name="value">The raw value fo the name</param>
        /// <param name="formatter">The formatter</param>
        public Name(string value, IFormatter formatter)
        {
            this.value = value;
            this.formatter = formatter;
        }

        /// <summary>
        /// Prefixes a string to a Name.
        /// </summary>
        /// <param name="str">The string to prefix</param>
        /// <param name="name">The name</param>
        /// <returns>A new name.</returns>
        public static Name operator +(string str, Name name) => new Name(str + " " + name.value, name.formatter);

        /// <summary>
        /// Suffixes a string to a Name.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="str"></param>
        /// <returns>A new name.</returns>
        public static Name operator +(Name name, string str) => new Name(name.value + " " + str, name.formatter);

        public static Name operator +(Name name1, Name name2) => new Name(name1.value + " " + name2.value, name1.formatter);

        /// <summary>
        /// Render the value of the name, using the formatter.
        /// </summary>
        /// <returns>A formatted string</returns>
        public override string ToString() => formatter is null ? value : formatter.Format(value);

        /// <summary>
        /// Render the value of the name, using a supplied formatter.
        /// This lets you override the default formatting of the name.
        /// </summary>
        /// <param name="formatter">The formatter to use.</param>
        /// <returns>A formatted string</returns>
        public string Format(Formatter formatter) => formatter.Format(value);
    }
    /*
    public delegate string NameFormatter(Name name);

    public class Name : IEquatable<Name>
    {
        static Regex identifier = new Regex(Properties.Resources.IdentifierRegex);

        static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        public static string UndefinedValue = Properties.Resources.UndefinedValue;

        public static string NameSeparator = " ";

        // Default formatter, used for casting to string
        public static NameFormatter Formatter = name => name.ApplyDefault().QuoteIfNeeded();

        // Reserved SQL keywords, used to control quoting.
        static ISet<string> reserved = new HashSet<string>(Properties.Resources.Reserved.Split(',').Select(r => r.ToLower().Trim()));

        public readonly string RawValue;

        public bool IsReserved { get { return reserved.Contains(RawValue.ToLower()); } }

        public bool NeedsQuotes { get { return IsReserved || !identifier.IsMatch(RawValue); } }

        public string Quote() => string.Format("[{0}]", RawValue);

        public string QuoteIfNeeded() => NeedsQuotes ? string.Format("[{0}]", RawValue) : RawValue;

        public Name ToPascalCase() => new Name(textInfo.ToTitleCase(RawValue).Replace(" ", ""));

        public Name NoSpaces() => new Name(RawValue.Replace(" ", ""));

        public Name ApplyDefault() => new Name(RawValue ?? UndefinedValue);

        public static Name operator +(string str, Name name) => new Name(str + NameSeparator + name.RawValue);

        public static Name operator +(Name name, string str) => new Name(name.RawValue + NameSeparator + str);

        public static Name operator +(Name name1, Name name2) => new Name(name1.RawValue + NameSeparator + name2.RawValue);

        // Name <-> string
        public Name(string rawName) => RawValue = rawName;

        public Name() : this(Name.UndefinedValue) { }

        public override string ToString()
        {
            var value = Formatter(this);
            if(!(value is string))
            {
                throw new StaticSQLException("Name.Formatter must return string.");
            }
            return "tostring:" + value.GetTypeCode().ToString() + value;
        }

        public static implicit operator Name(string rawName) => new Name(rawName);

        public static implicit operator string(Name name) => "implicit_cast:" + name.ToString();

        public bool Equals(Name other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return RawValue == other.RawValue;
        }


        public static bool operator ==(Name lhs, Name rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Name lhs, Name rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return RawValue.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals((Name)obj);
        }
    }
    */
}
