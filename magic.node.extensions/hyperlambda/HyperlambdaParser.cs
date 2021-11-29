/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using magic.node.extensions.hyperlambda.helpers;

namespace magic.node.extensions.hyperlambda
{
    /// <summary>
    /// Class that helps you parse Hyperlambda and create a Lambda/node from it.
    /// </summary>
    public static class HyperlambdaParser
    {
        /// <summary>
        /// Creates a Lambda/node from the specified string Hyperlambda.
        /// </summary>
        /// <param name="hyperlambda">Hyperlambda to parse</param>
        /// <param name="comments">Whether or not to include comments semantically in the resulting lambda or not</param>
        /// <returns>The node representation of the specified Hyperlambda</returns>
        public static Node Parse(string hyperlambda, bool comments = false)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(hyperlambda)))
            {
                var tokens = new HyperlambdaTokenizer(stream).Tokens();
                var result = new Node();
                BuildNodes(result, tokens, comments);
                return result;
            }
        }

        /// <summary>
        /// Creates a Lambda/node from the specified stream assumed to contain Hyperlambda.
        /// </summary>
        /// <param name="stream">Stream containing Hyperlambda to parse</param>
        /// <param name="comments">Whether or not to include comments semantically in the resulting lambda or not</param>
        /// <returns>The node representation of the specified Hyperlambda</returns>
        public static Node Parse(Stream stream, bool comments = false)
        {
            var tokens = new HyperlambdaTokenizer(stream).Tokens();
            var result = new Node();
            BuildNodes(result, tokens, comments);
            return result;
        }

        #region [ -- Private methods -- ]

        /*
         * Buildes nodes from the specified tokens, and puts these into the specified node.
         */
        static void BuildNodes(Node result, List<Token> tokens, bool keepComments)
        {
            // Current parent, implying which node to add the currently created node into.
            var currentParent = result;

            // The node we're currently handling.
            Node currentNode = null;

            // What level we're at, implying number of SP characters divided by 3.
            var level = 0;

            // Keeping track of previous token since it might influence how we handle current token, defaulting to CR/LF token.
            var previous = new Token(TokenType.CRLF, "\r\n");

            // Iterating through each tokens specified by caller.
            foreach (var idx in tokens)
            {
                switch (idx.Type)
                {
                    case TokenType.MultiLineComment:
                    case TokenType.SingleLineComment:
                        if (keepComments)
                        {
                            if (previous.Type == TokenType.CRLF)
                            {
                                // If previous token was CR/LF token, this token is a root level name declaration.
                                currentParent = result;
                                level = 0;
                            }
                            var cmt = new Node("..", idx.Value);
                            currentParent.Add(cmt);
                        }
                        break;

                    case TokenType.Name:
                        if (previous.Type == TokenType.CRLF)
                        {
                            // If previous token was CR/LF token, this token is a root level name declaration.
                            currentParent = result;
                            level = 0;
                        }
                        if (keepComments && currentParent.Name == "..")
                            throw new HyperlambdaException($"Tried to add a child node of a comment node having the value of {currentParent.Value}");
                        currentNode = new Node(idx.Value);
                        currentParent.Add(currentNode);
                        break;

                    case TokenType.Separator:
                        if (previous.Type == TokenType.Name && currentNode != null)
                            currentNode.Value = ""; // Defaulting value in case there are no more tokens.
                        break;

                    case TokenType.Type:
                        if (currentNode != null)
                            currentNode.Value = idx.Value;
                        break;

                    case TokenType.Value:
                        if (currentNode != null)
                        {
                            if (string.IsNullOrEmpty(currentNode.Get<string>()))
                                currentNode.Value = idx.Value;
                            else
                                currentNode.Value = Converter.ToObject(idx.Value, currentNode.Get<string>());
                        }
                        break;

                    case TokenType.CRLF:
                        break;

                    case TokenType.Space:
                        var scope = FindCurrentScope(idx.Value, level, currentParent);
                        level = scope.Item1;
                        currentParent = scope.Item2;
                        break;
                }
                previous = idx;
            }
        }

        /*
         * Finds current scope, and returns it as an integer, in addition
         * to returning its new parent to caller.
         */
        static (int, Node) FindCurrentScope(string token, int level, Node currentParent)
        {
            int newLevel = token.Length / 3;
            if (newLevel > level + 1)
            {
                // Syntax error in Hyperlambda, too many consecutive SP characters.
                throw new HyperlambdaException("Too many spaces found in Hyperlambda content");
            }
            if (newLevel == level + 1)
            {
                // Children collection opens up.
                currentParent = currentParent.Children.Last();
                level = newLevel;
            }
            else
            {
                // Propagating upwards in ancestor hierarchy.
                while (level > newLevel)
                {
                    currentParent = currentParent.Parent;
                    --level;
                }
            }
            return (level, currentParent);
        }

        #endregion
    }
}
