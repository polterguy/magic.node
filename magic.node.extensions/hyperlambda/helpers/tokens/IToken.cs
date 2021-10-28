/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers.tokens
{
    public enum TokenType { CRLF, MultiLineComment, Name, Separator, SingleLineComment, Space, Type, Value };

    public interface IToken
    {
        string Value { get; }

        TokenType Type { get; }
    }
}
