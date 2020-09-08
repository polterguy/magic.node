/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.IO;
using System.Text;
using System.Linq;
using magic.node.extensions.hyperlambda.internals;

namespace magic.node.extensions.hyperlambda
{
    /// <summary>
    /// Class allowing you to parse Hyperlambda from its textual representation,
    /// and create a lambda structure out of it.
    /// </summary>
    public sealed class Parser
    {
        readonly Node _root = new Node();

        /// <summary>
        /// Creates a new parser intended for parsing the specified Hyperlambda.
        /// </summary>
        /// <param name="hyperlambda">Hyperlambda you want to parse.</param>
        public Parser(string hyperlambda)
        {
            using (var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(hyperlambda))))
            {
                Parse(reader);
            }
        }

        /// <summary>
        /// Creates a new parser for Hyperlambda, parsing directly from the specified stream.
        /// </summary>
        /// <param name="stream">Stream to parse Hyperlambda from. Can be a forward
        /// only stream if necessary.</param>
        public Parser(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                Parse(reader);
            }
        }

        /// <summary>
        /// Returns the root lambda node parsed from your Hyperlambda.
        /// Notice, each node in your Hyperlambda at root level, will be a child
        /// node of the root node returned from this method.
        /// </summary>
        /// <returns>Root node containing all first level nodes from your Hyperlambda.</returns>
        public Node Lambda()
        {
            return _root;
        }

        #region [ -- Private helper methods -- ]

        void Parse(StreamReader reader)
        {
            var tokenizer = new Tokenizer(reader);
            var currentParent = _root;

            Node currentNode = null;
            string previousToken = null;
            int currentLevel = 0;

            foreach (var idxToken in tokenizer.GetTokens())
            {
                switch (idxToken)
                {
                    case ":":
                        var tuple = HandleColon(currentNode, currentParent, idxToken, previousToken);
                        previousToken = tuple.Item1;
                        currentNode = tuple.Item2;
                        break;

                    case "\r\n":
                        currentNode = null; // Making sure we create a new node on next iteration.
                        previousToken = idxToken;
                        break;

                    default:

                        // Checking if token is a scope declaration.
                        if (currentNode == null &&
                            idxToken.Any() &&
                            !idxToken.Any(x => x != ' '))
                        {
                            // Token is only SP. Hence, it's a scope declaration.
                            var levelDeclaration = FindCurrentScope(idxToken, currentLevel, currentParent);
                            currentLevel = levelDeclaration.Item1;
                            currentParent = levelDeclaration.Item2;
                        }
                        else
                        {
                            if (previousToken == "\r\n")
                            {
                                // We're back at root level.
                                currentParent = _root;
                                currentLevel = 0;
                            }

                            currentNode = DecorateNode(idxToken, currentNode, currentParent);
                        }
                        previousToken = idxToken;
                        break;
                }
            }
        }

        (string, Node) HandleColon(
            Node currentNode,
            Node currentParent,
            string idxToken,
            string previousToken)
        {
            if (currentNode == null)
            {
                currentNode = new Node();
                currentParent.Add(currentNode);
            }
            else if (previousToken == ":")
            {
                currentNode.Value = ":";
                return (previousToken, currentNode);
            }

            if (currentNode.Value == null)
                currentNode.Value = "";
            return (idxToken, currentNode);
        }

        /*
         * Finds current scope, and returns it as an integer, in addition
         * to returning its new parent to caller.
         */
        (int, Node) FindCurrentScope(string token, int level, Node currentParent)
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

        /*
         * Decorates the given node, and/or creates a new node, and returns
         * the node to caller.
         */
        Node DecorateNode(string token, Node currentNode, Node currentParent)
        {
            if (currentNode == null)
            {
                // First token for node.
                currentNode = new Node(token);
                currentParent.Add(currentNode);
            }
            else if (currentNode.Value == null || "".Equals(currentNode.Value))
            {
                // Second token for node.
                currentNode.Value = token;
            }
            else
            {
                // Third token for node, hence assuming previous token was type declaration.
                currentNode.Value = Converter.ToObject(token, currentNode.Get<string>());
            }
            return currentNode;
        }

        #endregion
    }
}
