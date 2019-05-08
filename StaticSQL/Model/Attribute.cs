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

        [JsonProperty("description")]
        public string Description;

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
                        FriendlyDataType = "integer";
                        break;

                    case "INT":
                    case "SMALLINT":
                    case "TINYINT":
                        DotNetDataType = "int";
                        FriendlyDataType = "integer";
                        break;

                    case "BIT":
                        DotNetDataType = "bool";
                        FriendlyDataType = "yes/no flag";
                        break;

                    case "UNIQUEIDENTIFIER":
                        DotNetDataType = "Guid";
                        FriendlyDataType = "unique identifier";
                        break;

                    case "BINARY":
                        DotNetDataType = "bool";
                        FriendlyDataType = "binary data";
                        break;

                    case "CHAR":
                    case "NCHAR":
                        FriendlyDataType = "fixed-length string";
                        DotNetDataType = "string";
                        break;

                    case "NVARCHAR":
                    case "VARCHAR":
                        DotNetDataType = "string";
                        FriendlyDataType = "string";
                        break;

                    case "DATE":
                        FriendlyDataType = "date";
                        DotNetDataType = "DateTime";
                        break;

                    case "DATETIME":
                    case "DATETIME2":
                        FriendlyDataType = "date and time";
                        DotNetDataType = "DateTime";
                        break;

                    case "DECIMAL":
                    case "NUMERIC":
                        FriendlyDataType = "decimal number";
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

        public string FriendlyDataType;

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
