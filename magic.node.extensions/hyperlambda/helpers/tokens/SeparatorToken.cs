/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers.tokens
{
    public class SeparatorToken : IToken
    {
        public string Value => ":";

        public TokenType Type => TokenType.Separator;
    }
}
