using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace StaticSQL
{
    public class Project
    {
        [JsonProperty("entities")]
        public readonly ICollection<string> EntityPaths = new List<string>();

        [JsonProperty("flows")]
        public readonly ICollection<string> FlowPaths = new List<string>();

        public ICollection<Entity> Entities = new List<Entity>();

        public ICollection<Flow> Flows = new List<Flow>();

        public string DirectoryName;

        public string FileName;

        public static Project Load(string path)
        {
            Project project = Load<Project>(path);

            project.DirectoryName = Path.GetDirectoryName(path);
            project.FileName = Path.GetFileName(path);

            foreach (string EntityPath in project.EntityPaths)
            {
                Entity Entity = Load<Entity>(Path.Combine(project.DirectoryName, EntityPath));
                project.Entities.Add(Entity);
            }

            foreach (string flowPath in project.FlowPaths)
            {
                Flow flow = Load<Flow>(Path.Combine(project.DirectoryName, flowPath));
                project.Flows.Add(flow);
            }

            return project;
        }

        static public T Load<T>(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            T Entity = JsonConvert.DeserializeObject<T>(text, new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            return Entity;
        }
    }
}