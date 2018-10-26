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

        [JsonProperty("is_nullable")]
        public bool IsNullable = false;

        public string NullabilityString { get { return IsNullable ? "NULL" : "NOT NULL"; } }

        public IDictionary<string, string> Ext = new Dictionary<string,string>();
    }
}