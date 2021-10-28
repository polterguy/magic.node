/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers.tokens
{
    public class SpaceToken : IToken
    {
        readonly string _spaces;

        public SpaceToken(string spaces)
        {
            _spaces = spaces;
        }

        public string Value => _spaces;

        public TokenType Type => TokenType.Space;
    }
}
