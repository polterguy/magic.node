/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using Xunit;
using magic.node.extensions;
using magic.node.expressions;
using magic.node.extensions.hyperlambda;
using System;

namespace magic.node.tests
{
    /*
     * Unit tests for lambda expressions.
     */
    public class ExpressionTests
    {
        [Fact]
        public void Simple()
        {
            // Creating an expression.
            var x = new Expression("foo/bar");

            // Asserts.
            Assert.Equal(2, x.Iterators.Count());
            Assert.Equal("foo", x.Iterators.First().Value);
            Assert.Equal("bar", x.Iterators.Skip(1).First().Value);
        }

        [Fact]
        public void Evaluate_01()
        {
            // Creating some example lambda to run our expression on.
            var hl = @"foo
   bar
   xxx
   bar";
            var lambda = new Parser(hl).Lambda().Children;

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/*/bar");
            var result = x.Evaluate(lambda.First()).ToList();

            // Asserts.
            Assert.Equal(2, result.Count());
            Assert.Equal("bar", result.First().Name);
            Assert.Equal("bar", result.Skip(1).First().Name);
        }

        [Fact]
        public void Evaluate_Throws()
        {
            // Notice, unless identity node's value is an expression, Evaluate will throw.
            Assert.Throws<ArgumentException>(() => new Node().Evaluate());
        }

        [Fact]
        public void Evaluate_02()
        {
            // Creating some example lambda to run our expression on.
            // Notice, making sure we use Mac OS X logic for carriage returns.
            var hl = "foo\n   bar1\n   bar2\nfoo\n   bar3";
            var lambda = new Parser(hl).Lambda();

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("*/foo/*");
            var result = x.Evaluate(lambda).ToList();

            // Asserts.
            Assert.Equal(3, result.Count());
            Assert.Equal("bar1", result.First().Name);
            Assert.Equal("bar2", result.Skip(1).First().Name);
            Assert.Equal("bar3", result.Skip(2).First().Name);
        }

        [Fact]
        public void Evaluate_03()
        {
            // Creating some example lambda to run our expression on.
            var hl = @"foo
   bar
   xxx
   bar";
            var lambda = new Parser(hl).Lambda().Children;

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("../0/**");
            var result = x.Evaluate(lambda.First()).ToList();

            // Asserts.
            Assert.Equal(4, result.Count());
            Assert.Equal("foo", result.First().Name);
            Assert.Equal("bar", result.Skip(1).First().Name);
            Assert.Equal("xxx", result.Skip(2).First().Name);
            Assert.Equal("bar", result.Skip(3).First().Name);
        }

        [Fact]
        public void Evaluate_04()
        {
            // Creating some example lambda to run our expression on.
            var hl = @"foo:node:@""foo:bar""";
            var lambda = new Parser(hl).Lambda();

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("../0/#");
            var result = x.Evaluate(lambda);

            // Asserts.
            Assert.Single(result);
            var str = result.First().ToHyperlambda();
            Assert.Equal("\"\"\r\n   foo:bar\r\n", str);
        }

        [Fact]
        public void ParentIterator()
        {
            // Creating some example lambda to run our expression on.
            var hl = "foo\n   bar\n   bar";
            var lambda = new Parser(hl).Lambda().Children;

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/*/bar/.");
            var result = x.Evaluate(lambda.First()).ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo", result.First().Name);
        }

        [Fact]
        public void VariableIterator()
        {
            // Creating some example lambda to run our expression on.
            var hl = "foo\n   bar\n   bar";
            var lambda = new Parser(hl).Lambda();

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/1/@foo");
            var result = x.Evaluate(lambda.Children.First()).ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("foo", result.First().Name);
        }

        [Fact]
        public void SubscriptIterator()
        {
            // Creating some example lambda to run our expression on.
            var hl = @"foo
   bar:error
   bar:success";
            var lambda = new Parser(hl).Lambda();

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/*/[1,1]");
            var result = x.Evaluate(lambda.Children.First()).ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("bar", result.First().Name);
            Assert.Equal("success", result.First().Value);
        }

        [Fact]
        public void NextIterator()
        {
            // Creating some example lambda to run our expression on.
            var hl = "foo\n   bar1\n   bar2";
            var lambda = new Parser(hl).Lambda().Children;

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/*/bar1/+");
            var result = x.Evaluate(lambda.First()).ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("bar2", result.First().Name);
        }

        [Fact]
        public void PreviousIterator()
        {
            // Creating some example lambda to run our expression on.
            var hl = "foo\n   bar1\n   bar2";
            var lambda = new Parser(hl).Lambda().Children;

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/*/bar2/-");
            var result = x.Evaluate(lambda.First()).ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("bar1", result.First().Name);
        }

        [Fact]
        public void PreviousIteratorRoundtrip()
        {
            // Creating some example lambda to run our expression on.
            var hl = "foo\n   bar1\n   bar2";
            var lambda = new Parser(hl).Lambda().Children;

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/*/bar1/-");
            var result = x.Evaluate(lambda.First()).ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("bar2", result.First().Name);
        }

        [Fact]
        public void NextIteratorRoundtrip()
        {
            // Creating some example lambda to run our expression on.
            var hl = "foo\n   bar1\n   bar2";
            var lambda = new Parser(hl).Lambda().Children;

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/*/bar2/+");
            var result = x.Evaluate(lambda.First()).ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("bar1", result.First().Name);
        }

        [Fact]
        public void EqualIterator_01()
        {
            // Creating some example lambda to run our expression on.
            var hl = "foo\n   bar1:xxx\n   bar1:yyy";
            var lambda = new Parser(hl).Lambda().Children;

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/*/bar1/=xxx");
            var result = x.Evaluate(lambda.First()).ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("bar1", result.First().Name);
            Assert.Equal("xxx", result.First().Value);
        }

        [Fact]
        public void EqualIteratorConversion_02()
        {
            // Creating some example lambda to run our expression on.
            var hl = "foo\n   bar1:int:5\n   bar1:yyy";
            var lambda = new Parser(hl).Lambda().Children;

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/*/bar1/=5");
            var result = x.Evaluate(lambda.First()).ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("bar1", result.First().Name);
        }

        [Fact]
        public void EqualIteratorSpacing_03()
        {
            // Creating some example lambda to run our expression on.
            var hl = "foo\n   bar1:hello world\n   bar1:yyy";
            var lambda = new Parser(hl).Lambda().Children;

            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/*/bar1/=hello world");
            var result = x.Evaluate(lambda.First()).ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("bar1", result.First().Name);
        }

        [Fact]
        public void EmptySequence_01()
        {
            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/1/@foo/*/..");

            // Evaluating our expression on an empty lambda object.
            var result = x.Evaluate(new Node()).ToList();

            // Asserts.
            Assert.Empty(result);
        }

        [Fact]
        public void NodeReferencingIteratorConstValue()
        {
            // Evaluating our expression such that the node is referencing another node.
            var hl = @"
foo:x:../*/{0}
   :.res
.res:OK";
            var lambda = new Parser(hl).Lambda();
            var identity = lambda.Children.First();
            var result = identity.Evaluate().ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("OK", result.First().Value);
        }

        [Fact]
        public void NodeReferencingIteratorExpressionValue_01()
        {
            // Evaluating our expression such that the node is referencing another node.
            var hl = @"
.val:.res
foo:x:../*/{0}
   :x:@.val
.res:OK";
            var lambda = new Parser(hl).Lambda();
            var identity = lambda.Children.Skip(1).First();
            var result = identity.Evaluate().ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("OK", result.First().Value);
        }

        [Fact]
        public void NodeReferencingIteratorExpressionValue_02()
        {
            // Evaluating our expression such that the node is referencing another node.
            var hl = @"
.val:.res
foo:x:../*/{0}
   :x:../*/.val
.res:OK";
            var lambda = new Parser(hl).Lambda();
            var identity = lambda.Children.Skip(1).First();
            var result = identity.Evaluate().ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("OK", result.First().Value);
        }

        [Fact]
        public void NodeReferencingIteratorExpressionValue_03()
        {
            // Evaluating our expression such that the node is referencing another node.
            var hl = @"
.val:.res
foo:x:../*/{0}
   :x:../*/{0}/./*/{1}
      :.val
      :x:../*/.val2
.val2:.val
.res:OK";
            var lambda = new Parser(hl).Lambda();
            var identity = lambda.Children.Skip(1).First();
            var result = identity.Evaluate().ToList();

            // Asserts.
            Assert.Single(result);
            Assert.Equal("OK", result.First().Value);
        }
    }
}
