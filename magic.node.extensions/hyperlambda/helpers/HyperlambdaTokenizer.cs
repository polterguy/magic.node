/*
* Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
* See the enclosed LICENSE file for details.
*/

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using magic.node.extensions.hyperlambda.internals;

namespace magic.node.extensions.hyperlambda.helpers
{
    /// <summary>
    /// Tokenizer for Hyperlambda allowing you to easily tokenize a snippet of Hyperlambda.
    /// </summary>
    public class HyperlambdaTokenizer
    {
        readonly StreamReader _reader;
        readonly List<Token> _tokens = new List<Token>();

        /// <summary>
        /// Creates an instance of your object, and reads all tokens from the specified stream
        /// </summary>
        /// <param name="stream">Stream object to read tokens from</param>
        public HyperlambdaTokenizer(Stream stream)
        {
            // Creating our stream reader which we're internally using to read tokens from the specified stream.
            _reader = new StreamReader(stream);

            // Looping until EOF of stream.
            while (!_reader.EndOfStream)
            {
                /*
                 * Initially, and after a value's CR/LF sequence, we can only have SP tokens, comments or names.
                 */
                while (true)
                {
                    // Skipping initial CR/LF sequences
                    ParserHelper.EatCRLF(_reader);

                    // Verifying we've still got more characters in stream.
                    if (_reader.EndOfStream)
                        break;

                    // Space tokens should only occur before names and comments.
                    ReadSpaceToken();

                    // Verifying we've still got more characters in stream.
                    if (_reader.EndOfStream)
                        break;

                    // The next token, if any, purely logically must be a name token, or a comment token.
                    ReadNameOrComment(out var isComment);

                    // Verifying we've still got more characters in stream.
                    if (_reader.EndOfStream)
                        break;

                    // Notice, we loop for as long as we find comments, to support multiple consecutive comments in the stream.
                    if (!isComment)
                        break;
                }

                // Verifying we've still got more characters in stream.
                if (_reader.EndOfStream)
                    break;

                // A separator might occur after a node's name.
                ReadSeparator();

                // Verifying we've still got more characters in stream.
                if (_reader.EndOfStream)
                    break;

                // A type or value might occur after a the separator following the node's name.
                if (ReadTypeOrValue() && !_reader.EndOfStream)
                {
                    // Above invocation returned type token, hence now reading value.
                    ReadSeparator();
                    if (!_reader.EndOfStream)
                        ReadTypeOrValue();
                }
 
                // Verifying we've still got more characters in stream.
                if (_reader.EndOfStream)
                    break;

                // A CR/LF sequence might occur after the node's name or value.
                ReadCRLF();
            }
        }

        /// <summary>
        /// Returns all tokens to the caller.
        /// </summary>
        /// <returns></returns>
        public List<Token> Tokens()
        {
            // Returning tokens to caller.
            return _tokens;
        }

        #region [ -- Private helper methods -- ]

        // Reads SP character and creates an SP token, if stream has SP characters at its current position.
        void ReadSpaceToken()
        {
            /*
             * Notice, we only add space tokens that have some character behind it, besides CR or LF characters.
             *
             * This makes the parser more resilient towards erronous spacings in our Hyperlambda files.
             */
            while (true)
            {
                if (_reader.EndOfStream)
                    return;
                var builder = new StringBuilder();
                var next = (char)_reader.Peek();
                while (next == ' ')
                {
                    builder.Append((char)_reader.Read());
                    if (_reader.EndOfStream)
                        return;
                    next = (char)_reader.Peek();
                }
                if (builder.Length > 0)
                {
                    if (next == '\n' || next == '\r')
                    {
                        _reader.Read();
                        while (true)
                        {
                            next = (char)_reader.Peek();
                            if (_reader.EndOfStream)
                                return;
                            if (next != '\r' && next != '\n')
                                break;
                            _reader.Read();
                        }
                    }
                    else
                    {
                        // Next character is not SP, '\r' or '\n'.
                        var spaces = builder.ToString();
                        if (spaces.Length % 3 != 0)
                            throw new ArgumentException($"Not correct number of spaces after:\r\n {string.Join("", _tokens.Select(x => x.Value))}");
                        _tokens.Add(new Token(TokenType.Space, spaces));
                        break;
                    }
                }
                else
                    break;
            }
        }

        // Reads the next name or comment in the stream.
        void ReadNameOrComment(out bool wasComment)
        {
            // Defaulting this to false, and only changing it if we're actually reading a comment.
            wasComment = false;

            // If the next character is a ':' character, this node has an empty name.
            if ((char)_reader.Peek() == ':')
            {
                _tokens.Add(new Token(TokenType.Name, "")); // Empty name
                wasComment = false;
                return;
            }

            // Reading the next TWO characters to figure out which type of token the next token in the stream actually is.
            var current = (char)_reader.Read();
            var next = (char)_reader.Peek();

            // Checking type of token, which varies according to the two first characters in the stream.
            if (current == '/' && next == '*')
            {
                // Multi line comment.
                wasComment = true;
                var comment = ParserHelper.ReadMultiLineComment(_reader);
                if (comment == null)
                {
                    if (_reader.EndOfStream)
                        throw new ArgumentException($"EOF encountered before end of multi line comment start after:\r\n {string.Join("", _tokens.Select(x => x.Value))}");
                }
                else
                {
                    _tokens.Add(new Token(TokenType.MultiLineComment, comment));
                    _tokens.Add(new Token(TokenType.CRLF, "\r\n"));
                }
            }
            else if (current == '/' && next == '/')
            {
                // Single line comment.
                _reader.Read(); // Discarding '/' character.
                var line = _reader.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(line))
                {
                    _tokens.Add(new Token(TokenType.SingleLineComment, line));
                    _tokens.Add(new Token(TokenType.CRLF, "\r\n"));
                }
                wasComment = true;
            }
            else if (current == '@' && next == '"')
            {
                // Multi line string.
                _reader.Read(); // Discarding '"' character.
                _tokens.Add(new Token(TokenType.Name, ParserHelper.ReadMultiLineString(_reader)));
            }
            else if (current == '"' || current == '\'')
            {
                // Single line string.
                _tokens.Add(new Token(TokenType.Name, ParserHelper.ReadQuotedString(_reader, current)));
            }
            else
            {
                // Normal node name, without quotes.
                var builder = new StringBuilder();
                builder.Append(current);
                while (true)
                {
                    next = (char)_reader.Peek();
                    if (_reader.EndOfStream || next == ':' || next == '\r' || next == '\n')
                        break;
                    builder.Append((char)_reader.Read());
                }
                _tokens.Add(new Token(TokenType.Name, builder.ToString()));
            }
        }

        // Reads the nedt separator (':' character) from the stream.
        void ReadSeparator()
        {
            if ((char)_reader.Peek() == ':')
            {
                _reader.Read();
                _tokens.Add(new Token(TokenType.Separator, ":"));
            }
        }

        // Reads the next type or value declaration from the stream.
        bool ReadTypeOrValue()
        {
            var current = (char)_reader.Peek();
            if (current == '\r' || current == '\n')
            {
                if (_tokens.LastOrDefault()?.Type == TokenType.Separator)
                    _tokens.Add(new Token(TokenType.Value, "")); // Empty string value (not null)
                return false;
            }
            _reader.Read();
            var next = (char)_reader.Peek();
            if (current == '@' && next == '"')
            {
                // Multi line string.
                _reader.Read(); // Skipping '"' character.
                _tokens.Add(new Token(TokenType.Value, ParserHelper.ReadMultiLineString(_reader)));
                if (!_reader.EndOfStream)
                {
                    next = (char)_reader.Peek();
                    if (next != '\r' && next != '\n')
                        throw new ArgumentException($"Garbage characters after:\r\n {string.Join("", _tokens.Select(x => x.Value))}");
                }
                return false;
            }
            else if (current == '"' || current == '\'')
            {
                // Single line string.
                _tokens.Add(new Token(TokenType.Value, ParserHelper.ReadQuotedString(_reader, current)));
                if (!_reader.EndOfStream)
                {
                    next = (char)_reader.Peek();
                    if (next != '\r' && next != '\n')
                        throw new ArgumentException($"Garbage characters after: {string.Join("", _tokens.Select(x => x.Value))}");
                }
                return false;
            }
            else
            {
                // Normal node name, without quotes.
                var builder = new StringBuilder();
                builder.Append(current);
                while (true)
                {
                    next = (char)_reader.Peek();
                    if (_reader.EndOfStream || next == '\n' || next == '\r' || next == ':')
                        break;
                    builder.Append((char)_reader.Read());
                }
                _tokens.Add(new Token(next == ':' ? TokenType.Type : TokenType.Value, builder.ToString().Trim()));
                return next == ':';
            }
        }

        // Reads the next CR/LF sequence from the stream.
        void ReadCRLF()
        {
            while(true)
            {
                var next = (char)_reader.Peek();
                if (_reader.EndOfStream || (next != '\n' && next != '\r'))
                    break;
                _reader.Read();
            }
            _tokens.Add(new Token(TokenType.CRLF, "\r\n"));
        }

        #endregion
    }
}
