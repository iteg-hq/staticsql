using System.Collections.Generic;
using Newtonsoft.Json;

namespace StaticSQL
{
    public class Attribute
    {
        [JsonProperty("tags")]
        public ISet<string> Tags = new HashSet<string>();

        [JsonProperty("name")]
        public Name Name;

        [JsonProperty("referenced_table")]
        public Name ReferencedTableName;

        [JsonProperty("data_type")]
        public string DataType;

        [JsonProperty("default")]
        public string DefaultValue;

        public bool HasDefault { get { return DefaultValue != null; } }

        [JsonProperty("computed_value")]
        public string ComputedValue;

        public bool IsComputed { get { return ComputedValue != null; } }

        [JsonProperty("is_nullable")]
        public bool IsNullable = false;

        public string NullabilityString { get { return IsNullable ? "NULL" : "NOT NULL"; } }

        public IDictionary<string, string> Ext = new Dictionary<string, string>();

        public Entity Entity;

        public int Index;

        public bool IsFirst { get { return Index == 0; } }

        public string CommaBefore { get { return IsFirst ? " " : ","; } }
    }
}
