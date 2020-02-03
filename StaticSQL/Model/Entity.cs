using System.Collections.Generic;
using Newtonsoft.Json;

namespace StaticSQL
{
    public class Entity
    {
        public string FilePath = null;

        [JsonProperty("schema")]
        public string RawSchemaName = Properties.Resources.UndefinedValue;

        public Name SchemaName { get => Project.GetName(RawSchemaName); }

        [JsonProperty("name")]
        public string RawName = Properties.Resources.UndefinedValue;

        public Name Name { get => Project.GetName(RawName); }

        [JsonProperty("description")]
        public string Description = "(No description)";

        [JsonProperty("source")]
        public string Source = Properties.Resources.UndefinedValue;

        [JsonProperty("tags")]
        public ISet<string> Tags = new HashSet<string>();

        [JsonProperty("attributes")]
        public IList<Attribute> Attributes = new List<Attribute>();

        [JsonProperty("data")]
        public IList<Dictionary<string, object>> Data = new List<Dictionary<string, object>>();

        public Project Project;
    }
}