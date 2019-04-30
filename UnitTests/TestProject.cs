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

        [TestMethod]
        public void TestType()
        {
            StaticSQL.Attribute attr = new StaticSQL.Attribute() { DataType = "INT" };
            Assert.AreEqual("INT", attr.SqlDataType);
            Assert.AreEqual("int", attr.DotNetDataType);
        }

        [TestMethod]
        public void TestTypeLength()
        {
            StaticSQL.Attribute attr = new StaticSQL.Attribute() { DataType = "NVARCHAR(10)" };
            Assert.AreEqual("NVARCHAR", attr.SqlDataType);
            Assert.AreEqual(10, attr.Length);
            Assert.AreEqual("string", attr.DotNetDataType);
        }

        [TestMethod]
        public void TestTypePrecision()
        {
            StaticSQL.Attribute attr = new StaticSQL.Attribute() { DataType = "DECIMAL(10,4)" };
            Assert.AreEqual("DECIMAL", attr.SqlDataType);
            Assert.AreEqual(10, attr.Length);
            Assert.AreEqual(4, attr.Precision);
            Assert.AreEqual("Decimal", attr.DotNetDataType);
        }
    }
}
