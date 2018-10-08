using System.Collections.Generic;
using Newtonsoft.Json;

namespace StaticSQL
{
    public class Column
    {
        [JsonProperty("tags")]
        public ISet<string> Tags = new HashSet<string>();

        [JsonProperty("name")]
        public string RawName;

        public string Name { get { return Util.QuoteIfNeeded(RawName); } }

        [JsonProperty("data_type")]
        public string DataType;

        [JsonProperty("is_nullable")]
        public bool IsNullable = false;
      
    }
}