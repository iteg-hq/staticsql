using System.Collections.Generic;
using Newtonsoft.Json;

namespace StaticSQL
{
    public class Entity
    {
        [JsonProperty("schema")]
        public Name SchemaName;

        [JsonProperty("name")]
        public Name Name;

        [JsonProperty("description")]
        public string Description = "(No description)";

        [JsonProperty("tags")]
        public ISet<string> Tags = new HashSet<string>();

        [JsonProperty("columns")]
        public IEnumerable<Attribute> Attributes = new List<Attribute>();

        public void TransformNames(NameFormatter nameTransform)
        {
            SchemaName = nameTransform(SchemaName);
            Name = nameTransform(Name);
            foreach(Attribute attribute in Attributes)
            {
                attribute.Name = nameTransform(attribute.Name);
            }
        }
    }
}