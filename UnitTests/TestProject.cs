﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StaticSQL;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class TestProject
    {
        public IFileSystem GetFileSystem()
        {
            MockFileSystem fs = new MockFileSystem();
            fs.AddFile(@"C:\test\test.staticsql", new MockFileData("{ \"entity_folder\": \"StaticSQL\"}"));

            fs.AddDirectory(@"C:\test\subfolder");


            fs.AddFile(@"C:\test\StaticSQL\entity.json", new MockFileData("{\"attributes\": \"\"}"));
            fs.AddDirectory(@"C:\empty");

            fs.AddFile(@"C:\double\one.staticsql", new MockFileData("{ \"entity_folder\": \"StaticSQL\"}"));
            fs.AddFile(@"C:\double\two.staticsql", new MockFileData("{ \"entity_folder\": \"StaticSQL\"}"));

            return fs;
        }


        [TestMethod]
        public void TestGetLocationsFolder()
        {
            var fs = GetFileSystem();
            IEnumerable<DirectoryInfoBase> locations = Project.GetSearchLocations(@"C:\test\subfolder", fs);
            Assert.AreEqual(@"C:\test\subfolder,C:\test,C:\", String.Join(",", locations.Select(d => d.FullName)));
        }

        public void TestGetLocationsPath()
        {
            var fs = GetFileSystem();
            IEnumerable<DirectoryInfoBase> locations = Project.GetSearchLocations(@"C:\test\subfolder\somefile.text", fs);
            Assert.AreEqual(@"C:\test\subfolder,C:\test,C:\", String.Join(",", locations.Select(d => d.FullName)));
        }

        [TestMethod]
        public void TestLoadFromPath()
        {
            var fs = GetFileSystem();
            var project = Project.Load(@"C:\test\test.staticsql", fs);
            Assert.IsNotNull(project);
            Assert.AreEqual(@"C:\test\StaticSQL", project.EntityFolderPath);
        }

        [TestMethod]
        public void TestLoadFromFolder()
        {
            var fs = GetFileSystem();
            var project = Project.Load(@"C:\test", fs);
            Assert.IsNotNull(project);
            Assert.AreEqual(@"C:\test\StaticSQL", project.EntityFolderPath);
        }

        [TestMethod]
        public void TestLoadFromSubFolder()
        {
            var fs = GetFileSystem();
            var project = Project.Load(@"C:\test\subfolder", fs);
            Assert.IsNotNull(project);
            Assert.AreEqual(@"C:\test\StaticSQL", project.EntityFolderPath);
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
            Assert.AreEqual("System.Int32", attr.DotNetDataType);
        }

        [TestMethod]
        public void TestTypeLength()
        {
            StaticSQL.Attribute attr = new StaticSQL.Attribute() { DataType = "NVARCHAR(10)" };
            Assert.AreEqual("NVARCHAR", attr.SqlDataType);
            Assert.AreEqual(10, attr.Length);
            Assert.AreEqual("System.String", attr.DotNetDataType);
        }

        [TestMethod]
        public void TestTypePrecision()
        {
            StaticSQL.Attribute attr = new StaticSQL.Attribute() { DataType = "DECIMAL(10,4)" };
            Assert.AreEqual("DECIMAL", attr.SqlDataType);
            Assert.AreEqual(10, attr.Length);
            Assert.AreEqual(4, attr.Precision);
            Assert.AreEqual("System.Decimal", attr.DotNetDataType);
        }

        [TestMethod]
        public void TestPascalCase()
        {
            Name name = new Name("foo bar", FormatterFactory.PascalCase());
            Assert.AreEqual("FooBar", name.ToString());
        }

        [TestMethod]
        public void TestFormatter()
        {
            Name name = new Name("tab le", new CombinedFormatter(FormatterFactory.PascalCase(), FormatterFactory.QuoteIfNeeded()));
            Assert.AreEqual("[TabLe]", name.ToString());
        }

        [TestMethod]
        public void TestPascalCaseQuoteIfNeeded()
        {
            Name name = new Name("tab le", FormatterFactory.PascalCaseQuoteIfNeeded());
            Assert.AreEqual("[TabLe]", name.ToString());
        }

        [TestMethod]
        public void TestCamelCase()
        {
            Name name = new Name("foo bar", FormatterFactory.CamelCase());
            Assert.AreEqual("fooBar", name.ToString());
        }

        /*
        [TestMethod]
        public void TestSnakeCase()
        {
            Name name = new Name("foo bar", FormatterFactory.CamelCase());
            Assert.AreEqual("foo_bar", name.ToString());
        }

        [TestMethod]
        public void TestScreamingSnakeCase()
        {
            Name name = new Name("foo bar", FormatterFactory.CamelCase());
            Assert.AreEqual("FOO_BAR", name.ToString());
        }
        */
    }

}
