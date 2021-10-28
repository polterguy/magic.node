/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers.tokens
{
    public class NameToken : IToken
    {
        readonly string _name;

        public NameToken(string name)
        {
            _name = name;
        }

        public string Value => _name;

        public TokenType Type => TokenType.Name;
    }
}
