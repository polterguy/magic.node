/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers.tokens
{
    public class CRLFToken : IToken
    {
        public string Value => "\r\n";

        public TokenType Type => TokenType.CRLF;
    }
}
