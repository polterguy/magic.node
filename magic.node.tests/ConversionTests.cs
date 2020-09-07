/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using Xunit;
using magic.node.extensions.hyperlambda;

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
            Assert.Equal(true, Parser.ConvertValue(true, "bool"));
        }

        [Fact]
        public void ConvertBoolFalse()
        {
            Assert.Equal(false, Parser.ConvertValue(false, "bool"));
        }

        [Fact]
        public void ConvertBoolTrue_String()
        {
            Assert.Equal(true, Parser.ConvertValue("true", "bool"));
        }

        [Fact]
        public void ConvertBoolFalse_String()
        {
            Assert.Equal(false, Parser.ConvertValue("false", "bool"));
        }
    }
}
