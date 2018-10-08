using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;

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