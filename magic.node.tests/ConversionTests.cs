/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using Xunit;
using magic.node.extensions.hyperlambda;
using System;

namespace magic.node.tests
{
    /*
     * Unit tests for lambda expressions.
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
            var date = new DateTime(2020, 12, 23, 23, 59, 11, DateTimeKind.Utc);
            var result = Converter.ToString(date);
            Assert.Equal("date", result.Item1);
            Assert.Equal("2020-12-23T23:59:11.000Z", result.Item2);
        }
    }
}
