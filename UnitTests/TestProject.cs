using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StaticSQL;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace UnitTests
{


    [TestClass]
    public class TestProject
    {
        public IFileSystem GetFileSystem()
        {
            MockFileSystem fs = new MockFileSystem();
            fs.AddFile(@"C:\test\test.staticsql", new MockFileData("{ \"project_folder\": \"StaticSQL\"}"));

            fs.AddDirectory(@"C:\test\subfolder");


            fs.AddFile(@"C:\test\StaticSQL\entity.json", new MockFileData("{\"attributes\": \"\"}"));
            fs.AddDirectory(@"C:\empty");

            fs.AddFile(@"C:\double\one.staticsql", new MockFileData("{ \"project_folder\": \"StaticSQL\"}"));
            fs.AddFile(@"C:\double\two.staticsql", new MockFileData("{ \"project_folder\": \"StaticSQL\"}"));

            return fs;
        }

        [TestMethod]
        public void TestLoadFromPath()
        {
            var fs = GetFileSystem();
            var project = Project.Load(@"C:\test\test.staticsql", fs);
            Assert.IsNotNull(project);
            Assert.AreEqual(@"C:\test\StaticSQL", project.ProjectFolderPath);
        }

        [TestMethod]
        public void TestLoadFromFolder()
        {
            var fs = GetFileSystem();
            var project = Project.Load(@"C:\test", fs);
            Assert.IsNotNull(project);
            Assert.AreEqual(@"C:\test\StaticSQL", project.ProjectFolderPath);
        }

        [TestMethod]
        public void TestLoadFromSubFolder()
        {
            var fs = GetFileSystem();
            var project = Project.Load(@"C:\test\subfolder", fs);
            Assert.IsNotNull(project);
            Assert.AreEqual(@"C:\test\StaticSQL", project.ProjectFolderPath);
        }

        [TestMethod]
        public void TestMissingProjectFile()
        {
            var fs = GetFileSystem();
            Assert.ThrowsException<StaticSQLException>(() => Project.Load(@"C:\empty", fs));
        }

        [TestMethod]
        public void TestMultipleProjectFile()
        {
            var fs = GetFileSystem();
            Assert.ThrowsException<StaticSQLException>(() => Project.Load(@"C:\double", fs));
        }
    }
}
