﻿using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.IO.Abstractions;
using System;

namespace StaticSQL
{
    public class Project
    {
        public string EntityFolderPath = ".";

        // The path of the project folder, relative to the folder of the project file.
        [JsonProperty("entity_folder")]
        internal readonly string relativeEntityFolderPath = ".";

        // Parameters for general use.
        [JsonProperty("parameters")]
        public readonly IDictionary<string, object> Parameters = new Dictionary<string, object>();

        // The file extension for entity files
        [JsonProperty("entity_extension")]
        public readonly string EntityExtension = "*.json";

        [JsonProperty("entities")]
        public IList<Entity> Entities = new List<Entity>();

        public IFormatter Formatter = FormatterFactory.PascalCase();

        public Name GetName(string value) => new Name(value, Formatter);

        public static Project Load(string path)
        {
            return Load(path, new FileSystem());
        }


        public static IEnumerable<DirectoryInfoBase> GetSearchLocations(string folder, IFileSystem fileSystem)
        {
            DirectoryInfoBase directory = fileSystem.DirectoryInfo.FromDirectoryName(folder);

            while (true)
            {
                if (directory is null) break;
                yield return directory;
                directory = directory.Parent;
            }
        }

        // Load a project from the project file at path.
        // If path is a directory, the directory and it parent directories will be searched for a project file.
        public static Project Load(string path, IFileSystem fileSystem)
        {
            Project project = null;
            DirectoryInfoBase folder = null;

            // If the path points to a file, load it
            if (fileSystem.File.Exists(path))
            {
                project = LoadJSON<Project>(path, fileSystem);
                FileInfoBase file = fileSystem.FileInfo.FromFileName(path);
                folder = file.Directory;
            }

            else if (fileSystem.Directory.Exists(path))
            {
                foreach (DirectoryInfoBase searchFolder in GetSearchLocations(path, fileSystem))
                {
                    FileInfoBase[] files = searchFolder.GetFiles(@"*.staticsql");
                    if(files.Length == 1)
                    {
                        FileInfoBase file = files.Single();
                        project = LoadJSON<Project>(file.FullName, fileSystem);
                        folder = file.Directory;
                        break;
                    }
                    else if (files.Length > 1)
                    {
                        throw new StaticSQLException("Multiple project files found " + string.Join(", ", files.Select(f => f.FullName)));
                    }
                }
            }

            if(folder is null)
            {
                throw new StaticSQLException("No project file found.");
            }

            project.EntityFolderPath = fileSystem.Path.Combine(folder.FullName, project.relativeEntityFolderPath);

            foreach (string entityPath in fileSystem.Directory.GetFiles(project.EntityFolderPath, ".\\*.json", System.IO.SearchOption.AllDirectories))
            {
                Entity entity = LoadJSON<Entity>(entityPath, fileSystem);
                entity.FilePath = entityPath;
                entity.Project = project;
                int index = 0;
                foreach (Attribute attr in entity.Attributes)
                {
                    attr.Entity = entity;
                    attr.Index = index;
                    index += 1;
                }
                project.Entities.Add(entity);
            }

            return project;


/*
            string originalPath = path;
            Project project = null;
            DirectoryInfoBase folder = null;

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
                        project = LoadJSON<Project>(files.Single().FullName, fileSystem);
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
                throw new StaticSQLException($"File not found: '{path}'");
            }


            return project;
            */
        }

        // Convenience method: Deserialize a json document.
        static private T LoadJSON<T>(string path, IFileSystem fileSystem)
        {
            string text = fileSystem.File.ReadAllText(path);
            try
            {
                return JsonConvert.DeserializeObject<T>(text);
            }
            catch (Exception exception)
            {
                throw new StaticSQLException($"Error loading ${path}", exception);
            }
        }
    }

    public class StaticSQLException : Exception
    {
        public StaticSQLException(string message) : base(message) { }
        public StaticSQLException(string message, Exception innerException) : base(message, innerException) { }
    }
}