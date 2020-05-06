using System.Collections.Generic;
using Newtonsoft.Json;

namespace StaticSQL
{
    public class Entity
    {
        public string FilePath = null;

        [JsonProperty("schema")]
        private readonly string rawSchemaName = Properties.Resources.UndefinedValue;

        [JsonProperty("name")]
        private readonly string rawName = Properties.Resources.UndefinedValue;

        public Name SchemaName { get => Project.GetName(rawSchemaName); }

        public Name Name { get => Project.GetName(rawName); }

        [JsonProperty("description")]
        public string Description = "(No description)";

        [JsonProperty("source")]
        public string Source = Properties.Resources.UndefinedValue;

        [JsonProperty("tags")]
        public TagSet Tags = new TagSet();

        [JsonProperty("attributes")]
        public IList<Attribute> Attributes = new List<Attribute>();

        [JsonProperty("data")]
        public IList<Dictionary<string, object>> Data = new List<Dictionary<string, object>>();

        public Project Project;
    }
}