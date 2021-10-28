/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers.tokens
{
    public class MultiLineCommentToken : IToken
    {
        readonly string _comment;

        public MultiLineCommentToken(string comment)
        {
            _comment = comment;
        }

        public string Value => _comment;

        public TokenType Type => TokenType.MultiLineComment;
    }
}
