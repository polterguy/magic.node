/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2021, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using magic.node.extensions.hyperlambda;

namespace magic.node.extensions
{
    /// <summary>
    /// Extension class extending the Node class with convenience methods.
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        /// Evaluates the expression found in the node's value, and returns
        /// the results of the evaluation.
        ///
        /// Notice, if the value of the node is not an expression, the method
        /// will throw an exception.
        /// </summary>
        /// <returns>Result of evaluation.</returns>
        public static IEnumerable<Node> Evaluate(this Node node)
        {
            if (!(node.Value is Expression ex))
                throw new ArgumentException($"'{node.Value}' is not a valid Expression, and hence Node cannot be evaluated.");

            return ex.Evaluate(node);
        }

        /// <summary>
        /// Returns the value of the node as typeof(T). This method will not
        /// evaluate any expressions, but rather return expressions as is,
        /// without evaluating expressions in any ways.
        /// </summary>
        /// <typeparam name="T">Type to return value as, which might imply
        /// conversion if value is not already of the specified type.</typeparam>
        /// <returns>The node's value as an object of type T</returns>
        public static T Get<T>(this Node node)
        {
            if (node.Value is T result)
                return result;

            // byte[] to string conversion support, and vice versa.
            if (typeof(T) == typeof(string) && node.Value is byte[] bytes)
                return (T)(object)Encoding.UTF8.GetString(bytes);
            if (typeof(T) == typeof(byte[]) && node.Value is string stringValue)
                return (T)(object)Encoding.UTF8.GetBytes(stringValue);

            // Converting, the simple version.
            return (T)Convert.ChangeType(node.Value, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Will return value of node as typeof(T), converting if necessary,
        /// and also evaluating any expressions found recursively, until a
        /// non-expression value is found.
        ///
        /// Notice, if expressions are evaluated, and the result of evaluating
        /// the expression finds multiple results, an exception will be thrown.
        /// </summary>
        /// <typeparam name="T">Type to return value or evaluated expression's
        /// value as.</typeparam>
        /// <param name="node">Node to retrieve value or evaluated expression
        /// from.</param>
        /// <returns>Value of node, or value of its evaluated expression.</returns>
        public static T GetEx<T>(this Node node)
        {
            // Checking if we're dealing with an expression, and if not, returning converted value.
            if (!(node.Value is Expression ex))
                return node.Get<T>();

            var value = ex.Evaluate(node);

            // Sanity checking result.
            if (value.Count() > 1)
                throw new ArgumentException("Multiple resulting nodes from expression.");

            // Making sure we return default value for T if we have no result.
            if (!value.Any())
                return default;

            // Returning result of evaluation process.
            // Notice, this process will recursively evaluate all expressions, 
            // if one expression leads to another expression, etc.
            return value.First().GetEx<T>();
        }

        /// <summary>
        /// Transforms the specified node into its Hyperlambda syntax.
        /// </summary>
        /// <param name="node">Nod to transform into Hyperlambda</param>
        /// <returns></returns>
        public static string ToHyperlambda(this Node node)
        {
            return HyperlambdaGenerator.GetHyperlambda(new Node[] { node });
        }
    }
}
