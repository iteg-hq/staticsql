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
    public class Table
    {
        [JsonProperty("schema")]
        private string RawSchemaName = "";

        public string SchemaName { get { return RawSchemaName; } }

        [JsonProperty("name")]
        public string RawName;

        [JsonProperty("description")]
        public string Description = "(No description)";

        public string Name { get { return Util.QuoteIfNeeded(RawName); } }

        public string QuotedName { get { return Name; } }

        [JsonProperty("tags")]
        public ISet<string> Tags = new HashSet<string>();

        [JsonProperty("columns")]
        public IEnumerable<Column> Columns = new List<Column>();

        static public Table Load(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            Table table = JsonConvert.DeserializeObject<Table>(text);
            return table;
        }
    }
}