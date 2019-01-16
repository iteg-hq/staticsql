using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.IO.Abstractions;
using System;

namespace StaticSQL
{
    public class Project
    {
        public string ProjectFolderPath = ".";

        // The path of the project folder, relative to the folder of the project file.
        [JsonProperty("project_folder")]
        internal readonly string relativeProjectFolderPath = ".";

        // Parameters for general use.
        [JsonProperty("parameters")]
        public readonly IDictionary<string, object> Parameters = new Dictionary<string, object>();

        // The file extension for entity files
        [JsonProperty("entity_extension")]
        public readonly string EntityExtension = "*.json";

        public IList<Entity> Entities = new List<Entity>();

        public static Project Load(string path)
        {
            return Load(path, new FileSystem());
        }

        // Load a project from the project file at path.
        // If path is a directory, the directory and it parent directories will be searched for a project file.
        public static Project Load(string path, IFileSystem fileSystem)
        {
            string originalPath = path;
            Project project = null;
            DirectoryInfoBase folder = null;
            if (fileSystem.File.Exists(path))
            {
                project = LoadJSON<Project>(path, fileSystem);
                folder = fileSystem.FileInfo.FromFileName(path).Directory;
            }

            else if (fileSystem.Directory.Exists(path))
            {
                folder = fileSystem.DirectoryInfo.FromDirectoryName(path);
                FileInfoBase[] files;
                List<string> folders = new List<string>();
                do
                {
                    // Climb the directory tree, looking for a single .staticsql file.
                    folders.Add(folder.FullName);
                    files = folder.GetFiles(@"*.staticsql");
                    if (files.Length == 1)
                    {
                        project = Project.LoadJSON<Project>(files.Single().FullName, fileSystem);
                        break;
                    }
                    else if (files.Length > 1)
                    {
                        throw new StaticSQLException("Multiple project files found " + String.Join(", ", files.Select(f => f.FullName)));
                    }

                    try
                    {
                        folder = fileSystem.Directory.GetParent(path);
                        path = folder.FullName;
                    }
                    catch
                    {
                        throw new StaticSQLException("Project file not found in: " + String.Join(", ", folders));
                    }
                } while (project == null);
            }
            else
            {
                throw new System.Exception();
            }

            project.ProjectFolderPath = fileSystem.Path.Combine(folder.FullName, project.relativeProjectFolderPath);

            foreach (string entityPath in fileSystem.Directory.GetFiles(project.ProjectFolderPath, ".\\*.json", System.IO.SearchOption.AllDirectories))
            {
                Entity entity = Project.LoadJSON<Entity>(entityPath, fileSystem);
                entity.AfterLoad();
                project.Entities.Add(entity);
            }


            return project;
        }

        // Convenience method: Deserialize a json document.
        static private T LoadJSON<T>(string path, IFileSystem fileSystem)
        {
            string text = fileSystem.File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(text);
        }
    }

    public class StaticSQLException : Exception
    {
        public StaticSQLException(string message) : base(message) { }
    }
}