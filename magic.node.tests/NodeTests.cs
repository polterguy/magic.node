/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System.Linq;
using Xunit;

namespace magic.node.tests
{
    /*
     * Unit tests for lambda expressions.
     */
    public class NodeTests
    {
        [Fact]
        public void NoName()
        {
            var n = new Node();
            Assert.Equal("", n.Name);
            Assert.Null(n.Value);
            Assert.Empty(n.Children);
        }

        [Fact]
        public void OnlyName()
        {
            var n = new Node("foo");
            Assert.Equal("foo", n.Name);
            Assert.Null(n.Value);
            Assert.Empty(n.Children);
        }

        [Fact]
        public void NameValue()
        {
            var n = new Node("foo", "bar");
            Assert.Equal("foo", n.Name);
            Assert.Equal("bar", n.Value);
            Assert.Empty(n.Children);
        }

        [Fact]
        public void NameValueChildren()
        {
            var n = new Node("foo", "bar", new Node[] { new Node("howdy") });
            Assert.Equal("foo", n.Name);
            Assert.Equal("bar", n.Value);
            Assert.Single(n.Children);
            Assert.Equal("howdy", n.Children.First().Name);
            Assert.Equal("foo", n.Children.First().Parent.Name);
        }

        [Fact]
        public void Add()
        {
            var n = new Node("parent");
            n.Add(new Node("foo"));
            Assert.Equal("foo", n.Children.First().Name);
            Assert.Equal("parent", n.Children.First().Parent.Name);
        }

        [Fact]
        public void AddFromOtherNode()
        {
            var n = new Node("parent");
            n.Add(new Node("foo"));
            var n2 = new Node("parent2");
            n2.Add(n.Children.First());
            Assert.Equal("foo", n2.Children.First().Name);
            Assert.Equal("parent2", n2.Children.First().Parent.Name);
            Assert.Empty(n.Children);
        }

        [Fact]
        public void InsertBefore()
        {
            var n = new Node("parent");
            n.Add(new Node("foo"));
            n.Children.First().InsertBefore(new Node("bar"));
            Assert.Equal("bar", n.Children.First().Name);
            Assert.Equal("foo", n.Children.Skip(1).First().Name);
            Assert.Equal("parent", n.Children.First().Parent.Name);
            Assert.Equal("parent", n.Children.Skip(1).First().Parent.Name);
        }

        [Fact]
        public void InsertBeforeFromOther()
        {
            var n1 = new Node("parent1");
            n1.Add(new Node("foo1"));

            var n2 = new Node("parent2");
            n2.Add(new Node("foo2"));

            n2.Children.First().InsertBefore(n1.Children.First());

            Assert.Equal("foo1", n2.Children.First().Name);
            Assert.Equal("foo2", n2.Children.Skip(1).First().Name);
            Assert.Equal("parent2", n2.Children.First().Parent.Name);
            Assert.Equal("parent2", n2.Children.Skip(1).First().Parent.Name);
            Assert.Empty(n1.Children);
        }

        [Fact]
        public void InsertAfter()
        {
            var n = new Node("parent");
            n.Add(new Node("foo"));
            n.Children.First().InsertAfter(new Node("bar"));
            Assert.Equal("foo", n.Children.First().Name);
            Assert.Equal("bar", n.Children.Skip(1).First().Name);
            Assert.Equal("parent", n.Children.First().Parent.Name);
            Assert.Equal("parent", n.Children.Skip(1).First().Parent.Name);
        }

        [Fact]
        public void InsertAfterFromOther()
        {
            var n1 = new Node("parent1");
            n1.Add(new Node("foo1"));

            var n2 = new Node("parent2");
            n2.Add(new Node("foo2"));

            n2.Children.First().InsertAfter(n1.Children.First());

            Assert.Equal("foo2", n2.Children.First().Name);
            Assert.Equal("foo1", n2.Children.Skip(1).First().Name);
            Assert.Equal("parent2", n2.Children.First().Parent.Name);
            Assert.Equal("parent2", n2.Children.Skip(1).First().Parent.Name);
            Assert.Empty(n1.Children);
        }

        [Fact]
        public void AddRangeFromOther()
        {
            var n1 = new Node("parent1", null, new Node[] { new Node("foo1"), new Node("foo2") });
            var n2 = new Node("parent2");
            n2.AddRange(n1.Children);
            Assert.Empty(n1.Children);
            Assert.Equal(2, n2.Children.Count());
        }

        [Fact]
        public void Clear()
        {
            var n1 = new Node("parent");
            var n2 = new Node();
            n1.Add(n2);
            var n3 = new Node();
            n1.Add(n3);
            n1.Clear();
            Assert.Empty(n1.Children);
            Assert.Null(n2.Parent);
            Assert.Null(n3.Parent);
        }

        [Fact]
        public void Untie()
        {
            var n1 = new Node("parent");
            var n2 = new Node();
            n1.Add(n2);
            var n3 = new Node();
            n1.Add(n3);
            n1.Children.First().UnTie();
            Assert.Single(n1.Children);
            n1.Children.First().UnTie();
            Assert.Empty(n1.Children);
            Assert.Null(n2.Parent);
            Assert.Null(n3.Parent);
        }

        [Fact]
        public void Next()
        {
            var n1 = new Node("parent1", null, new Node[] { new Node("foo1"), new Node("foo2") });
            Assert.Equal("foo2", n1.Children.First().Next.Name);
        }

        [Fact]
        public void Previous()
        {
            var n1 = new Node("parent1", null, new Node[] { new Node("foo1"), new Node("foo2") });
            Assert.Equal("foo1", n1.Children.Skip(1).First().Previous.Name);
        }
    }
}
