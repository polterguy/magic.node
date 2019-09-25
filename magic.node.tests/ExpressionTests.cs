/*
 * Magic, Copyright(c) Thomas Hansen 2019 - thomas@gaiasoul.com
 * Licensed as Affero GPL unless an explicitly proprietary license has been obtained.
 */

using System.Linq;
using Xunit;
using magic.node.expressions;
using magic.node.extensions.hyperlambda;

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
            var hl = "foo\n   bar\n   xxx\n   bar";
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
        public void Evaluate_02()
        {
            // Creating some example lambda to run our expression on.
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
        public void EqualIterator_02()
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
        public void EmptySequence_01()
        {
            // Creating an expression, and evaluating it on above lambda.
            var x = new Expression("foo/1/@foo/*/..");

            // Evaluating our expression on an empty lambda object.
            var result = x.Evaluate(new Node()).ToList();

            // Asserts.
            Assert.Empty(result);
        }
    }
}
