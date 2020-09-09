/*
 * Magic, Copyright(c) Thomas Hansen 2019 - 2020, thomas@servergardens.com, all rights reserved.
 * See the enclosed LICENSE file for details.
 */

using System;
using System.Linq;
using System.Collections.Generic;

namespace magic.node.extensions
{
    /// <summary>
    /// A single iterator component for an expression.
    /// </summary>
    public class Iterator
    {
        readonly Func<Node, IEnumerable<Node>, IEnumerable<Node>> _evaluate;

        /*
         * Dictionary containing lookups for non-parametrized iterators, implying an iterator that
         * doesn't require arguments, but becomes an exact match for the iterator evaluator. Such
         * as e.g. "..", "*", etc.
         */
        static readonly Dictionary<string, Func<Node, IEnumerable<Node>, IEnumerable<Node>>> _nonParametrizedIterators =
            new Dictionary<string, Func<Node, IEnumerable<Node>, IEnumerable<Node>>>
        {
            {"*", (identity, input) => input.SelectMany(x => x.Children)},
            {"#", (identity, input) => input.Select(x => x.Value as Node)},
            {"-", (identity, input) => input.Select(x => x.Previous ?? x.Parent.Children.Last())},
            {"+", (identity, input) => input.Select(x => x.Next ?? x.Parent.Children.First())},
            {".", (identity, input) => input.Select(x => x.Parent).Distinct()},
            {"**", (identity, input) => AllDescendants(input)},
            {"..", (identity, input) => {

                /*
                 * Notice, input might be a "no sequence enumerable",
                 * in case previous iterator yielded no results,
                 * so we'll have to accommodate for such cases.
                 */
                var idx = input.FirstOrDefault();
                if (idx == null)
                    return Array.Empty<Node>();

                while (idx.Parent != null)
                    idx = idx.Parent;

                return new Node[] { idx };
            }},
        };

        /*
         * Dictionary containing lookups for first character of parametrized iterator, resolving
         * to functor returning functor responsible for executing parametrized iterator.
         *
         * A parametrized iterator, is an iterator that somehow requires parameters,
         * such as arguments, declaring input arguments to the iterator.
         *
         * E.g. [0,1] is a parametrized iterator.
         */
        static readonly Dictionary<char, Func<string, Func<Node, IEnumerable<Node>, IEnumerable<Node>>>> _parametrizedIterators =
            new Dictionary<char, Func<string, Func<Node, IEnumerable<Node>, IEnumerable<Node>>>>
        {
            /*
             * Extrapolation iterator, resolving to name comparison, where name
             * to compare against, is fetched from its n'th children.
             */
            {'{', (value) => {
                var index = int.Parse(value.Substring(1, value.Length - 2));
                return (identity, input) => ExtrapolationIterator(
                    identity,
                    input,
                    index);
            }},

            /*
             * Value iterator, comparing the value of the iterator, with the
             * value of the node, converting to string before doing comparison,
             * if necessary.
             */
            {'=', (value) => {
                var name = value.Substring(1);
                return (identity, input) => ValueEqualsIterator(
                    input,
                    name);
            }},

            /*
             * Subscript iterator, returning from [n1 to n2> from its previous result set.
             */
            {'[', (value) => {
                var ints = value.Substring(1, value.Length - 2).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var start = int.Parse(ints[0]);
                var count = int.Parse(ints[1]);
                return (identity, input) => SubscriptIterator(
                    input,
                    start,
                    count);
            }},

            /*
             * Name equality iterator, returning the first node matching the specified name,
             * upwards in hierarchy, implying direct ancestors, and older sibling nodes,
             * and older siblings of direct ancestors.
             */
            {'@', (value) => {
                var name = value.Substring(1);
                return (identity, input) => AncestorNameIterator(input, name);
            }},
        };

        /*
         * Creates an iterator from its given string representation.
         */
        internal Iterator(string value)
        {
            Value = value;
            _evaluate = CreateEvaluator(Value);
        }

        /// <summary>
        /// Returns the string representation of the iterator.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Evaluates the iterator from the given identity node, with the given input,
        /// resulting in a new node set being returned by the evaluation.
        /// </summary>
        /// <param name="identity">Identity node from which the original expression was evaluated from.</param>
        /// <param name="input">A collection of nodes passed in from the result of the evaluation of the previous iterator.</param>
        /// <returns>An enumerable collection of Nodes, from the result of evaluating the iterator.</returns>
        public IEnumerable<Node> Evaluate(Node identity, IEnumerable<Node> input)
        {
            return _evaluate(identity, input);
        }

        /// <summary>
        /// Allows you to inject a non-parametrized iterator into the available
        /// iterators.
        /// </summary>
        /// <param name="iteratorValue">Fully qualified name, to match your functor.</param>
        /// <param name="functor">Functor to execute as your iterator name is matched.</param>
        public static void AddStaticIterator(
            string iteratorValue,
            Func<Node, IEnumerable<Node>, IEnumerable<Node>> functor)
        {
            _nonParametrizedIterators[iteratorValue] = functor;
        }

        /// <summary>
        /// Allows you to add a parametrized iterator into the available
        /// iterators.
        /// </summary>
        /// <param name="iteratorFirstCharacter">First character your iterator must start with.</param>
        /// <param name="createFunctor">Function that is responsible for creating your actual iterator.</param>
        public static void AddDynamicIterator(
            char iteratorFirstCharacter,
            Func<string, Func<Node, IEnumerable<Node>, IEnumerable<Node>>> createFunctor)
        {
            _parametrizedIterators[iteratorFirstCharacter] = createFunctor;
        }

        #region [ -- Overrides -- ]

        /// <summary>
        /// Returns a string representation of your Iterator.
        /// </summary>
        /// <returns>A string representation of your iterator.</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Returns the hash code for your instance.
        /// </summary>
        /// <returns>Hash code, useful for for instance creating keys for dictionaries, etc.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Comparison method, comparing the current instance to some other instance.
        /// </summary>
        /// <param name="obj">Right hand side to compare instance with.</param>
        /// <returns>True if instances are logically similar.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Iterator it))
                return false;

            return Value.Equals(it.Value);
        }

        #endregion

        #region [ -- Private helper methods -- ]

        /*
         * Creates the evaluator, which is simply a function, taking an identity node, an enumerable of
         * nodes, resuolting in a new enumerable of nodes.
         */
        static Func<Node, IEnumerable<Node>, IEnumerable<Node>> CreateEvaluator(string iteratorValue)
        {
            // If iterator is escaped, we assume it's a name lookup.
            if (iteratorValue.StartsWith("\\", StringComparison.InvariantCulture))
            {
                var name = iteratorValue.Substring(1);
                return (identity, input) => input.Where(x => x.Name == name);
            }

            /*
             * First checking non-parametrized iterators for a match of entire string.
             */
            if (_nonParametrizedIterators.ContainsKey(iteratorValue))
                return _nonParametrizedIterators[iteratorValue];

            /*
             * Only if there are no non-parametrized iterators matches, we resort
             * to looking into parametrized iterators for a match, but only if iterator
             * value is not empty string.
             */
            if (iteratorValue.Length != 0)
            {
                var firstCharacter = iteratorValue[0];
                if (_parametrizedIterators.ContainsKey(firstCharacter))
                    return _parametrizedIterators[firstCharacter](iteratorValue);

                /*
                 * Checking if this is an "n'th child iterator", which is true if
                 * its content can be successfully converted into an integer.
                 */
                if (int.TryParse(iteratorValue, out int number))
                    return (identity, input) => input.SelectMany(x => x.Children.Skip(number).Take(1));
            }

            /*
             * Defaulting to name lookup iterator.
             *
             * This is the default iterator, doing a basic name lookup, and the iterator
             * used if none of the above results in any match.
             */
            return (identity, input) => input.Where(x => x.Name == iteratorValue);
        }

        /*
         * Helper method to recursively evaluate a node.
         */
        static string EvaluateNode(Node node)
        {
            if (node == null)
                return "";
            if (node.Value is Expression exp)
            {
                var expResult = exp.Evaluate(node);
                if (expResult.Count() > 1)
                    throw new ArgumentException("Expression yielded multiple results when maximum one was expected");
                return EvaluateNode(expResult.FirstOrDefault());
            }
            return node.Value?.ToString() ?? "";
        }

        /*
         * Helper method to return all descendants recursively for the '**' iterator.
         */
        static IEnumerable<Node> AllDescendants(IEnumerable<Node> input)
        {
            foreach (var idx in input)
            {
                yield return idx;
                foreach (var idxInner in AllDescendants(idx.Children))
                {
                    yield return idxInner;
                }
            }
        }

        /*
         * Implementation of extrapolation iterator, resulting in name equality,
         * where name to look for is resolved as the n'th child of the identity node's
         * value, converted to string, if necessary, evaluating node if necessary.
         */
        static IEnumerable<Node> ExtrapolationIterator(
            Node identity,
            IEnumerable<Node> input,
            int index)
        {
            var node = identity.Children.Skip(index).First();
            return input.Where(x => x.Name.Equals(EvaluateNode(node)));
        }

        /*
         * Name equality iterator, requiring a statically declared name, returning
         * results of all nodes from previous result set, matching name specified.
         */
        static IEnumerable<Node> ValueEqualsIterator(
            IEnumerable<Node> input,
            string name)
        {
            return input.Where(x => {
                if (x.Value == null)
                    return name.Length == 0; // In case we're looking for null values

                if (x.Value is string)
                    return name.Equals(x.Value);

                return name.Equals(Converter.ToString(x.Value).Item2);
            });
        }

        /*
         * Subscript iterator, returning a subscript of the previous result set.
         */
        static IEnumerable<Node> SubscriptIterator(
            IEnumerable<Node> input,
            int start,
            int count)
        {
            return input.Skip(start).Take(count);
        }

        /*
         * Ancestor/elder-sibling iterator, returning the first node being either
         * a direct ancestor, or an older sibling (of self or direct ancestors),
         * matching the specified name.
         */
        static IEnumerable<Node> AncestorNameIterator(
            IEnumerable<Node> input,
            string name)
        {
            var cur = input.FirstOrDefault()?.Previous ?? input.FirstOrDefault()?.Parent;
            while (cur != null && cur.Name != name)
            {
                var previous = cur.Previous;
                if (previous == null)
                    cur = cur.Parent;
                else
                    cur = previous;
            }

            if (cur == null)
                return new Node[] { };

            return new Node[] { cur };
        }

        #endregion
    }
}
