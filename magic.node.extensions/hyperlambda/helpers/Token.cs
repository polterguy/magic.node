/*
* Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
* See the enclosed LICENSE file for details.
*/

namespace magic.node.extensions.hyperlambda.helpers
{
    /// <summary>
    /// Token type declaring which token type the token actually is.
    /// </summary>
    public enum TokenType { CRLF, MultiLineComment, Name, Separator, SingleLineComment, Space, Type, Value };

    /// <summary>
    /// A single Hyperlambda token.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Creates an instance of your token.
        /// </summary>
        /// <param name="type">Type of token</param>
        /// <param name="value">String representation of token</param>
        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Returns the string representation value of your token.
        /// </summary>
        /// <value>Token value</value>
        public string Value { get; }

        /// <summary>
        /// Returns the token type.
        /// </summary>
        /// <value>Token type</value>
        public TokenType Type { get; }

        #region [ -- Overridden base class methods -- ]

        public override string ToString()
        {
            return Type + ":" + Value;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion
    }
}
