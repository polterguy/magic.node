/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using Xunit;
using System;
using magic.node.expressions;
using magic.node.extensions.hyperlambda;

namespace magic.node.tests
{
    /*
     * Unit tests for converting from Hyperlambda string declarations to objects, and vice versa.
     */
    public class ConversionTests
    {
        [Fact]
        public void ConvertBoolTrue()
        {
            Assert.Equal(true, Converter.ToObject(true, "bool"));
        }

        [Fact]
        public void ConvertBoolFalse()
        {
            Assert.Equal(false, Converter.ToObject(false, "bool"));
        }

        [Fact]
        public void ConvertBoolTrue_String()
        {
            Assert.Equal(true, Converter.ToObject("true", "bool"));
        }

        [Fact]
        public void ConvertBoolFalse_String()
        {
            Assert.Equal(false, Converter.ToObject("false", "bool"));
        }

        [Fact]
        public void ConvertToString_01()
        {
            var result = Converter.ToString("Howdy World");
            Assert.Equal("string", result.Item1);
            Assert.Equal("Howdy World", result.Item2);
        }

        [Fact]
        public void ConvertToString_02()
        {
            var result = Converter.ToString("Howdy\r\nWorld");
            Assert.Equal("string", result.Item1);
            Assert.Equal("Howdy\r\nWorld", result.Item2);
        }

        [Fact]
        public void ConvertToString_03()
        {
            var result = Converter.ToString("Howdy\"World");
            Assert.Equal("string", result.Item1);
            Assert.Equal("Howdy\"World", result.Item2);
        }

        [Fact]
        public void ConvertToString_04()
        {
            var result = Converter.ToString("Howdy:World");
            Assert.Equal("string", result.Item1);
            Assert.Equal("Howdy:World", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromInt16()
        {
            var result = Converter.ToString((short)5);
            Assert.Equal("short", result.Item1);
            Assert.Equal("5", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromUInt16()
        {
            var result = Converter.ToString((ushort)5);
            Assert.Equal("ushort", result.Item1);
            Assert.Equal("5", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromInt32()
        {
            var result = Converter.ToString(5);
            Assert.Equal("int", result.Item1);
            Assert.Equal("5", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromUInt32()
        {
            var result = Converter.ToString((uint)5);
            Assert.Equal("uint", result.Item1);
            Assert.Equal("5", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromInt64()
        {
            var result = Converter.ToString(5L);
            Assert.Equal("long", result.Item1);
            Assert.Equal("5", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromUInt64()
        {
            var result = Converter.ToString((ulong)5);
            Assert.Equal("ulong", result.Item1);
            Assert.Equal("5", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromDecimal()
        {
            var result = Converter.ToString((decimal)5);
            Assert.Equal("decimal", result.Item1);
            Assert.Equal("5", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromDouble()
        {
            var result = Converter.ToString((double)5);
            Assert.Equal("double", result.Item1);
            Assert.Equal("5", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromFloat()
        {
            var result = Converter.ToString((float)5);
            Assert.Equal("float", result.Item1);
            Assert.Equal("5", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromDateTime()
        {
            var result = Converter.ToString(new DateTime(2020, 12, 23, 23, 59, 11, DateTimeKind.Utc));
            Assert.Equal("date", result.Item1);
            Assert.Equal("2020-12-23T23:59:11.000Z", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromTimeSpan()
        {
            var result = Converter.ToString(new TimeSpan(2000));
            Assert.Equal("time", result.Item1);
            Assert.Equal("2000", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromGuid()
        {
            var result = Converter.ToString(Guid.Empty);
            Assert.Equal("guid", result.Item1);
            Assert.Equal("00000000-0000-0000-0000-000000000000", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromChar()
        {
            var result = Converter.ToString('q');
            Assert.Equal("char", result.Item1);
            Assert.Equal("q", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromByte()
        {
            var result = Converter.ToString((byte)5);
            Assert.Equal("byte", result.Item1);
            Assert.Equal("5", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromExpression()
        {
            var result = Converter.ToString(new Expression("foo/bar"));
            Assert.Equal("x", result.Item1);
            Assert.Equal("foo/bar", result.Item2);
        }

        [Fact]
        public void ConvertToStringFromNode()
        {
            var node = new Node("foo");
            node.Add(new Node("howdy1", 5));
            node.Add(new Node("howdy2", 7M));
            var result = Converter.ToString(new Node("", null, new Node[] { node }));
            Assert.Equal("node", result.Item1);
            Assert.Equal(@"foo
   howdy1:int:5
   howdy2:decimal:7
", result.Item2);
        }

        class Foo
        {
            public int Value1 { get; set; }

            public string Value2 { get; set; }
        }

        [Fact]
        public void ConvertToStringFromCustomType()
        {
            Converter.AddConverter(
                typeof(Foo),
                "foo",
                (obj) => {
                    var fooInput = obj as Foo;
                    return ("foo", $"{fooInput.Value1},{fooInput.Value2}");
                }, (obj) => {
                    var strEntities = (obj as string).Split(',');
                    return new Foo
                    {
                        Value1 = int.Parse(strEntities[0]),
                        Value2 = strEntities[1],
                    };
                });
            var foo = new Foo
            {
                Value1 = 5,
                Value2 = "Howdy World",
            };
            var result = Converter.ToString(foo);
            Assert.Equal("foo", result.Item1);
            Assert.Equal("5,Howdy World", result.Item2);
        }

        [Fact]
        public void ConvertToCustomTypeFromString()
        {
            Converter.AddConverter(
                typeof(Foo),
                "foo",
                (obj) => {
                    var fooInput = obj as Foo;
                    return ("foo", $"{fooInput.Value1},{fooInput.Value2}");
                }, (obj) => {
                    var strEntities = (obj as string).Split(',');
                    return new Foo
                    {
                        Value1 = int.Parse(strEntities[0]),
                        Value2 = strEntities[1],
                    };
                });
            var result = Converter.ToObject("5,Howdy World", "foo") as Foo;
            Assert.Equal(5, result.Value1);
            Assert.Equal("Howdy World", result.Value2);
        }

        [Fact]
        public void CreateHyperlambdaWithCustomType()
        {
            Converter.AddConverter(
                typeof(Foo),
                "foo",
                (obj) => {
                    var fooInput = obj as Foo;
                    return ("foo", $"{fooInput.Value1},{fooInput.Value2}");
                }, (obj) => {
                    var strEntities = (obj as string).Split(',');
                    return new Foo
                    {
                        Value1 = int.Parse(strEntities[0]),
                        Value2 = strEntities[1],
                    };
                });
            var node = new Node();
            node.Add(new Node("some-value", 5));
            node.Add(new Node("foo", new Foo {
                Value1 = 5,
                Value2 = "Howdy World",
            }));
            node.Add(new Node("some-other-value", 5M));
            var hyperlambda = Generator.GetHyper(node.Children);
            Assert.Equal(@"some-value:int:5
foo:foo:5,Howdy World
some-other-value:decimal:5
", hyperlambda);
        }

        [Fact]
        public void CreateHyperlambdaWithCustomType_WithDoubleQuotes()
        {
            Converter.AddConverter(
                typeof(Foo),
                "foo",
                (obj) => {
                    var fooInput = obj as Foo;
                    return ("foo", $"{fooInput.Value1},{fooInput.Value2}");
                }, (obj) => {
                    var strEntities = (obj as string).Split(',');
                    return new Foo
                    {
                        Value1 = int.Parse(strEntities[0]),
                        Value2 = strEntities[1],
                    };
                });
            var node = new Node();
            node.Add(new Node("some-value", 5));
            node.Add(new Node("foo", new Foo
            {
                Value1 = 5,
                Value2 = "Howdy \"World",
            }));
            node.Add(new Node("some-other-value", 5M));
            var hyperlambda = Generator.GetHyper(node.Children);
            Assert.Equal(@"some-value:int:5
foo:foo:" + @"""5,Howdy \""World""" + @"
some-other-value:decimal:5
", hyperlambda);
        }
    }
}
