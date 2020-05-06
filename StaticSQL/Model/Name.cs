namespace StaticSQL
{

    public class Name
    {
        public readonly string Value;

        public readonly FormatterDelegate Formatter;

        public readonly FormatterDelegate Quoter;

        public Name(string value, FormatterDelegate formatter, FormatterDelegate quoter)
        {
            Value = value;
            Formatter = formatter;
            Quoter = quoter;
        }

        public static Name operator +(string str, Name name) => new Name(str + name.Value, name.Formatter, name.Quoter);

        public static Name operator +(Name name, string str) => new Name(name.Value + str, name.Formatter, name.Quoter);

        public static Name operator +(Name name1, Name name2) => new Name(name1.Value + name2.Value, name1.Formatter, name1.Quoter);

        public override string ToString() => Quoter(Formatter(Value));
    }
}
