/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using Xunit;
using magic.node.extensions.hyperlambda;
using System.Linq.Expressions;
using System.IO;
using System.Text;

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
        public void ReadFromStream()
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("foo")))
            {
                // Creating some lambda object.
                var result = new Parser(stream).Lambda().Children.ToList();

                // Asserts.
                Assert.Single(result);
                Assert.Equal("foo", result.First().Name);
                Assert.Null(result.First().Value);
                Assert.Empty(result.First().Children);
            }
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
            var result = new Parser(@"foo1
   bar
foo2
").Lambda().Children.ToList();

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
            var result = new Parser(@"foo1
   bar1
      bar2
   bar3").Lambda().Children.ToList();

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
        public void SpacingError_Throws()
        {
            // Should throw, too few spaces in front of "bar1".
            Assert.Throws<ArgumentException>(() => new Parser("foo1\r\n bar1"));
        }

        [Fact]
        public void AlphaInValue()
        {
            // Creating some lambda object.
            var result = new Parser(@"
howdy
world:foo@bar.com").Lambda();

            // Asserts.
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("world", result.Children.Skip(1).First().Name);
            Assert.Equal("foo@bar.com", result.Children.Skip(1).First().Value);
        }

        [Fact]
        public void SingleQuoteInValue()
        {
            // Creating some lambda object.
            var result = new Parser(@"
howdy
world:foo'bar.com").Lambda();

            // Asserts.
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("world", result.Children.Skip(1).First().Name);
            Assert.Equal("foo'bar.com", result.Children.Skip(1).First().Value);
        }

        [Fact]
        public void MultiLineString_01()
        {
            // Creating some lambda object.
            var result = new Parser("foo1:@\"howdy\r\nworld \"").Lambda().Children.ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo1", result.First().Name);
            Assert.Equal("howdy\r\nworld ", result.First().Value);
        }

        [Fact]
        public void MultiLineString_02()
        {
            // Creating some lambda object.
            var result = new Parser("\r\nhowdy\r\nworld:@\"howdy\r\nworld\"").Lambda();

            // Asserts.
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("world", result.Children.Skip(1).First().Name);
            Assert.Equal("howdy\r\nworld", result.Children.Skip(1).First().Value);
        }

        [Fact]
        public void DoubleQuoteInValue()
        {
            // Creating some lambda object.
            var result = new Parser(@"
howdy
world:foo""bar.com").Lambda();

            // Asserts.
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("world", result.Children.Skip(1).First().Name);
            Assert.Equal("foo\"bar.com", result.Children.Skip(1).First().Value);
        }

        [Fact]
        public void EOFBeforeDone()
        {
            // Creating some lambda object.
            Assert.Throws<ArgumentException>(() => new Parser(@"
howdy
world:""howdy world throws"));
        }

        [Fact]
        public void CharactersAfterClosingDoubleQuote()
        {
            // Creating some lambda object.
            Assert.Throws<ArgumentException>(() => new Parser(@"
howdy
world:""howdy world""throws"));
        }

        [Fact]
        public void ReadSingleLineComment()
        {
            // Creating some lambda object.
            var result = new Parser(@"
howdy
// This is a comment, and should be ignored!
world:foo""bar.com").Lambda();

            // Asserts.
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("world", result.Children.Skip(1).First().Name);
            Assert.Equal("foo\"bar.com", result.Children.Skip(1).First().Value);
        }

        [Fact]
        public void ReadSingleLineCommentNOTComment_01()
        {
            // Creating some lambda object.
            var result = new Parser(@"
howdy
!// This is a comment, and should NOT be ignored!:foo
world:foo").Lambda();

            // Asserts.
            Assert.Equal(3, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("!// This is a comment, and should NOT be ignored!", result.Children.Skip(1).First().Name);
            Assert.Equal("foo", result.Children.Skip(1).First().Value);
            Assert.Equal("world", result.Children.Skip(2).First().Name);
            Assert.Equal("foo", result.Children.Skip(2).First().Value);
        }

        [Fact]
        public void ReadSingleLineCommentNOTComment_02()
        {
            // Creating some lambda object.
            var result = new Parser(@"
howdy
/ This is a comment, and should NOT be ignored!:foo
world:foo").Lambda();

            // Asserts.
            Assert.Equal(3, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("/ This is a comment, and should NOT be ignored!", result.Children.Skip(1).First().Name);
            Assert.Equal("foo", result.Children.Skip(1).First().Value);
            Assert.Equal("world", result.Children.Skip(2).First().Name);
            Assert.Equal("foo", result.Children.Skip(2).First().Value);
        }

        [Fact]
        public void ReadMultiLineComment_01()
        {
            // Creating some lambda object.
            var result = new Parser(@"
howdy
/* This is a comment, and should be ignored! */
world:foo""bar.com").Lambda();

            // Asserts.
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("world", result.Children.Skip(1).First().Name);
            Assert.Equal("foo\"bar.com", result.Children.Skip(1).First().Value);
        }

        [Fact]
        public void ReadMultiLineComment_02()
        {
            // Creating some lambda object.
            var result = new Parser(@"
howdy
/*
 * This is a comment, and should be ignored!
 */
world:foo""bar.com").Lambda();

            // Asserts.
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("world", result.Children.Skip(1).First().Name);
            Assert.Equal("foo\"bar.com", result.Children.Skip(1).First().Value);
        }

        [Fact]
        public void ReadMultiLineComment_03_Throws()
        {
            // Creating some lambda object.
            Assert.Throws<ArgumentException>(() => new Parser(@"
howdy
/*
 * This is a comment, and should be ignored!
 * Next line will make sure this throws!
 * /
world:foo""bar.com"));
        }

        [Fact]
        public void BadCRLF_Throws_01()
        {
            // Creating some lambda object.
            Assert.Throws<ArgumentException>(() => new Parser("foo\r"));
        }

        [Fact]
        public void BadCRLF_Throws_02()
        {
            // Creating some lambda object.
            Assert.Throws<ArgumentException>(() => new Parser("foo\r "));
        }

        [Fact]
        public void ReadOddSpacesThrows_01()
        {
            // Creating some lambda object.
            Assert.Throws<ArgumentException>(() => new Parser(@"
foo
   // Next line throws!
  bar
"));
        }

        [Fact]
        public void ReadOddSpacesThrows_02()
        {
            // Creating some lambda object.
            Assert.Throws<ArgumentException>(() => new Parser(@"
foo
   // Next line throws!
    bar
"));
        }

        [Fact]
        public void ReadNonClosedMultiLine_Throws()
        {
            // Creating some lambda object.
            Assert.Throws<ArgumentException>(() => new Parser(@"
foo:@""howdy world
"));
        }

        [Fact]
        public void ReadSingleLineContainingCR_Throws()
        {
            // Creating some lambda object.
            Assert.Throws<ArgumentException>(() => new Parser(@"
foo:""howdy world
howdy""
"));
        }

        [Fact]
        public void ReadMultiLineContainingDoubleQuotes()
        {
            // Creating some lambda object.
            var result = new Parser(@"
howdy
world:@""foobar """" howdy""").Lambda();

            // Asserts.
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("world", result.Children.Skip(1).First().Name);
            Assert.Equal("foobar \" howdy", result.Children.Skip(1).First().Value);
        }

        [Fact]
        public void ReadSingleLineWithEscapeCharacter()
        {
            // Creating some lambda object.
            var result = new Parser(@"
howdy
world:""foobar \t howdy""").Lambda();

            // Asserts.
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("world", result.Children.Skip(1).First().Name);
            Assert.Equal("foobar \t howdy", result.Children.Skip(1).First().Value);
        }

        [Fact]
        public void ReadSingleLineWithEscapedHexCharacter()
        {
            // Creating some lambda object.
            var result = new Parser("\r\nhowdy\r\nworld:\"\"foobar \xfefe howdy").Lambda();

            // Asserts.
            Assert.Equal(2, result.Children.Count());
            Assert.Equal("howdy", result.Children.First().Name);
            Assert.Null(result.Children.First().Value);
            Assert.Equal("world", result.Children.Skip(1).First().Name);
            Assert.Equal("foobar \xfefe howdy", result.Children.Skip(1).First().Value);
        }
    }
}
