/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers.tokens
{
    public class ValueToken : IToken
    {
        readonly string _value;

        public ValueToken(string value)
        {
            _value = value;
        }

        public string Value => _value;

        public TokenType Type => TokenType.Value;
    }
}
