/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System;
using System.Linq;
using Xunit;
using magic.node.extensions.hyperlambda;

namespace magic.node.tests
{
    /*
     * Unit tests for Hyperlambda parser.
     */
    public class HyperlambdaParserTests
    {
        [Fact]
        public void Empty()
        {
            // Creating some lambda object.
            var result = new Parser("").Lambda().Children.ToList();

            // Asserts.
            Assert.Empty(result);
        }

        [Fact]
        public void SingleNode()
        {
            // Creating some lambda object.
            var result = new Parser("foo").Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo", result.First().Name);
            Assert.Null(result.First().Value);
            Assert.Empty(result.First().Children);
        }

        [Fact]
        public void NodeWithEmptyValue()
        {
            // Creating some lambda object.
            var result = new Parser("foo:").Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo", result.First().Name);
            Assert.Equal("", result.First().Value);
            Assert.Empty(result.First().Children);
        }

        [Fact]
        public void NodeWithValue()
        {
            // Creating some lambda object.
            var result = new Parser("foo:bar").Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo", result.First().Name);
            Assert.Equal("bar", result.First().Value);
            Assert.Empty(result.First().Children);
        }

        [Fact]
        public void NodeWithColonValue()
        {
            // Creating some lambda object.
            var result = new Parser(@"foo:"":""").Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo", result.First().Name);
            Assert.Equal(":", result.First().Value);
            Assert.Empty(result.First().Children);
        }

        [Fact]
        public void NodeWithTypedValue()
        {
            // Creating some lambda object.
            var result = new Parser("foo:int:5").Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo", result.First().Name);
            Assert.Equal(5, result.First().Value);
            Assert.Empty(result.First().Children);
        }

        [Fact]
        public void TwoRootNodes()
        {
            // Creating some lambda object.
            var result = new Parser("foo1:bar1\r\nfoo2:bar2").Lambda().Children.ToList();

            // Asserts.
            Assert.Equal(2, result.Count());
            Assert.Equal("foo1", result.First().Name);
            Assert.Equal("bar1", result.First().Value);
            Assert.Empty(result.First().Children);
            Assert.Equal("foo2", result.Skip(1).First().Name);
            Assert.Equal("bar2", result.Skip(1).First().Value);
            Assert.Empty(result.Skip(1).First().Children);
        }

        [Fact]
        public void NodeWithChildren()
        {
            // Creating some lambda object.
            var result = new Parser("foo\r\n   bar").Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo", result.First().Name);
            Assert.Null(result.First().Value);
            Assert.Single(result.First().Children);
            Assert.Equal("bar", result.First().Children.First().Name);
            Assert.Null(result.First().Children.First().Value);
        }

        [Fact]
        public void TwoRootNodesWithChildren()
        {
            // Creating some lambda object.
            var result = new Parser("foo1\r\n   bar\r\nfoo2").Lambda().Children.ToList();

            // Asserts.
            Assert.Equal(2, result.Count);
            Assert.Equal("foo1", result.First().Name);
            Assert.Null(result.First().Value);
            Assert.Single(result.First().Children);
            Assert.Equal("bar", result.First().Children.First().Name);
            Assert.Null(result.First().Children.First().Value);
            Assert.Equal("foo2", result.Skip(1).First().Name);
            Assert.Null(result.Skip(1).First().Value);
            Assert.Empty(result.Skip(1).First().Children);
        }

        [Fact]
        public void ComplexHierarchy()
        {
            // Creating some lambda object.
            var result = new Parser("foo1\r\n   bar1\r\n      bar2\r\n   bar3").Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo1", result.First().Name);
            Assert.Null(result.First().Value);
            Assert.Equal(2, result.First().Children.Count());
            Assert.Equal("bar1", result.First().Children.First().Name);
            Assert.Null(result.First().Children.First().Value);
            Assert.Equal("bar2", result.First().Children.First().Children.First().Name);
            Assert.Null(result.First().Children.First().Children.First().Value);
            Assert.Equal("bar3", result.First().Children.Skip(1).First().Name);
        }

        [Fact]
        public void DoubleQuotedString()
        {
            // Creating some lambda object.
            var result = new Parser(@"foo1:"" howdy world """).Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo1", result.First().Name);
            Assert.Equal(" howdy world ", result.First().Value);
        }

        [Fact]
        public void SingleQuotedString()
        {
            // Creating some lambda object.
            var result = new Parser("foo1:' howdy world '").Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo1", result.First().Name);
            Assert.Equal(" howdy world ", result.First().Value);
        }

        [Fact]
        public void MultilineString()
        {
            // Creating some lambda object.
            var result = new Parser("foo1:@\" howdy\r\nworld \"").Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo1", result.First().Name);
            Assert.Equal(" howdy\r\nworld ", result.First().Value);
        }

        [Fact]
        public void SpacingError_Throws()
        {
            // Should throw, too few spaces in front of "bar1".
            Assert.Throws<ApplicationException>(() => new Parser("foo1\r\n bar1"));
        }
    }
}
