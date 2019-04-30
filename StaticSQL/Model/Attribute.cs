using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        private string _datatype;

        public string SqlDataType;
        public int Length;
        public int Precision;

        private static Regex typeNameRegex = new Regex(@"^(\w+)(?:\s*\(\s*(\d+)\s*(?:,\s*(\d+)\s*)?\))?$");

        [JsonProperty("data_type")]
        public string DataType
        {
            set
            {
                _datatype = value;

                var m = typeNameRegex.Match(value);
                if (m.Success)
                {
                    SqlDataType = m.Groups[1].Value.ToUpper();
                    int.TryParse(m.Groups[2].Value, out Length);
                    int.TryParse(m.Groups[3].Value, out Precision);
                }
                switch(SqlDataType){
                    case "BIGINT":
                        DotNetDataType = "long";
                        break;

                    case "INT":
                    case "SMALLINT":
                    case "TINYINT":
                        DotNetDataType = "int";
                        break;

                    case "UNIQUEIDENTIFIER":
                        DotNetDataType = "Guid";
                        break;

                    case "BINARY":
                        DotNetDataType = "bool";
                        break;

                    case "CHAR":
                    case "NCHAR":
                    case "NVARCHAR":
                    case "VARCHAR":
                        DotNetDataType = "string";
                        break;

                    case "DATE":
                    case "DATETIME":
                    case "DATETIME2":
                        DotNetDataType = "DateTime";
                        break;

                    case "DECIMAL":
                    case "NUMERIC":
                        DotNetDataType = "Decimal";
                        break;
                }
            }

            get
            {
                return _datatype;
            }
        }

        public string DotNetDataType;

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
