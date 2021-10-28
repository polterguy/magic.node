/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers.tokens
{
    public class TypeToken : IToken
    {
        readonly string _typename;

        public TypeToken(string typename)
        {
            _typename = typename;
        }

        public string Value => _typename;

        public TokenType Type => TokenType.Type;
    }
}
