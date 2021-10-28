/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers
{
    public enum TokenType { CRLF, MultiLineComment, Name, Separator, SingleLineComment, Space, Type, Value };

    public class Token
    {
        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Value { get; }

        public TokenType Type { get; }
    }
}
