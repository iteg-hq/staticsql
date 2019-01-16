using System.Collections.Generic;
using Newtonsoft.Json;

namespace StaticSQL
{
    public class Entity
    {
        [JsonProperty("schema")]
        public Name SchemaName = Properties.Resources.UndefinedValue;

        [JsonProperty("name")]
        public Name Name = Properties.Resources.UndefinedValue;

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

        internal void AfterLoad()
        {
            int index = 0;
            foreach(Attribute attr in Attributes)
            {
                attr.Entity = this;
                attr.Index = index;
                index += 1;
            }
        }
    }
}