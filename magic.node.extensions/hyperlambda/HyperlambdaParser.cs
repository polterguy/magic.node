/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using magic.node.extensions.hyperlambda.helpers;

namespace magic.node.extensions.hyperlambda
{
    public static class HyperlambdaParser
    {
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

        public static Node Parse(Stream stream, bool comments = false)
        {
            var tokens = new HyperlambdaTokenizer(stream).Tokens();
            var result = new Node();
            BuildNodes(result, tokens, comments);
            return result;
        }

        #region [ -- Private methods -- ]

        static void BuildNodes(Node result, List<Token> tokens, bool keepComments)
        {
            var currentParent = result;
            Node currentNode = null;
            var level = 0;
            var previousWasCRLF = false;
            var previousWasName = false;
            foreach (var idx in tokens)
            {
                switch (idx.Type)
                {
                    case TokenType.CRLF:
                        previousWasCRLF = true;
                        break;

                    case TokenType.Separator:
                        if (previousWasName)
                            currentNode.Value = ""; // Defaulting value in case there are no more tokens.
                        break;

                    case TokenType.MultiLineComment:
                        if (keepComments)
                            currentParent.Add(new Node("..", idx.Value));
                        break;

                    case TokenType.SingleLineComment:
                        if (keepComments)
                            currentParent.Add(new Node("..", idx.Value));
                        break;

                    case TokenType.Name:
                        if (previousWasCRLF)
                        {
                            currentParent = result;
                            level = 0;
                        }
                        previousWasName = true;
                        currentNode = new Node(idx.Value);
                        currentParent.Add(currentNode);
                        break;

                    case TokenType.Space:
                        previousWasCRLF = false;
                        var scope = FindCurrentScope(idx.Value, level, currentParent);
                        level = scope.Item1;
                        currentParent = scope.Item2;
                        break;

                    case TokenType.Type:
                    previousWasName = false;
                        currentNode.Value = idx.Value;
                        break;

                    case TokenType.Value:
                        if (string.IsNullOrEmpty(currentNode.Get<string>()))
                            currentNode.Value = idx.Value;
                        else
                            currentNode.Value = Converter.ToObject(idx.Value, currentNode.Get<string>());
                        break;
                }
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
                throw new ArgumentException("Too many spaces found in Hyperlambda content");
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
