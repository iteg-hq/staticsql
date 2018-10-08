using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace StaticSQL
{
    public class Project
    {
        [JsonProperty("tables")]
        public readonly ICollection<string> TablePaths = new List<string>();

        [JsonProperty("reserved")]
        public readonly ICollection<string> ReservedWords = new List<string>();

        public ICollection<Table> Tables = new List<Table>();

        public string DirectoryName;

        public string FileName;

        public static Project Load(string path)
        {
            string text = System.IO.File.ReadAllText(path);
            Project project = JsonConvert.DeserializeObject<Project>(text);

            project.DirectoryName = Path.GetDirectoryName(path);
            project.FileName = Path.GetFileName(path);

            foreach (string tablePath in project.TablePaths)
            {
                Table table = Table.Load(Path.Combine(project.DirectoryName, tablePath));
                project.Tables.Add(table);
            }
            return project;
        }
    }
}