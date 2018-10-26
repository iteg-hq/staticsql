using System.Collections.Generic;
using Newtonsoft.Json;

namespace StaticSQL
{
    public class Flow
    {
        [JsonProperty("tags")]
        public ISet<string> Tags = new HashSet<string>();

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("execution_group_code")]
        public string ExecutionGroupCode = "None";

        [JsonProperty("database")]
        public string DatabaseName = "$(DatabaseName)";

        [JsonProperty("schema")]
        public string SchemaName = "flow";

        [JsonProperty("statuses")]
        public ICollection<Status> Statuses = new List<Status>();   
    }

    public class Status
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("is_active")]
        public bool IsActive = false;

        [JsonProperty("is_blocking")]
        public bool IsBlocking = false;

        [JsonProperty("is_success")]
        public bool IsSuccess = false;

        [JsonProperty("actions")]
        public ICollection<Action> Actions = new List<Action>();
    }

    public class Action
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("resulting_status")]
        public string ResultingStatusName;

        [JsonProperty("has_action_procedure")]
        public bool HasActionProcedure = false;

    }
}