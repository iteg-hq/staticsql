using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StaticSQL;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class TestName
    {
        [TestMethod]
        public void TestPascalCase()
        {
            Name name = new Name("foo bar", Formatting.PascalCase, Quoting.Never);
            Assert.AreEqual("FooBar", name.ToString());
        }

        [TestMethod]
        public void TestCamelCase()
        {
            Name name = new Name("foo bar", Formatting.CamelCase, Quoting.Never);
            Assert.AreEqual("fooBar", name.ToString());
        }

        [TestMethod]
        public void TestSnakeCase()
        {
            Name name = new Name("foo bar", Formatting.SnakeCase, Quoting.Never);
            Assert.AreEqual("foo_bar", name.ToString());
        }

        [TestMethod]
        public void TestFormattingOrder()
        {
            Name name = new Name("tab le", Formatting.NoSpaceCase, Quoting.AsNeeded);
            Assert.AreEqual("[table]", name.ToString());
        }

        [TestMethod]
        public void TestNamePlusString()
        {
            Name name = new Name("foo", Formatting.NoFormatting, Quoting.Never);
            Assert.AreEqual("foobar", (name + "bar").ToString());
        }

        [TestMethod]
        public void TestQuoteWhenNeeded()
        {
            Name name = new Name("Table", Formatting.NoFormatting, Quoting.AsNeeded);
            Assert.AreEqual("[Table]", name.ToString());
            Assert.AreEqual("LoadTable", ("Load" + name).ToString());
        }

    }

}
