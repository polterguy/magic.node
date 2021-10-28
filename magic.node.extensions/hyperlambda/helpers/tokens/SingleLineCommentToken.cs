/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers.tokens
{
    public class SingleLineCommentToken : IToken
    {
        readonly string _comment;

        public SingleLineCommentToken(string comment)
        {
            _comment = comment;
        }

        public string Value => _comment;

        public TokenType Type => TokenType.SingleLineComment;
    }
}
