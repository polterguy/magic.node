/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

namespace magic.node.contracts
{
    /// <summary>
    /// Contract for resolving root folder on disc for magic.lambda.io
    /// </summary>
    public interface IRootResolver
    {
        /// <summary>
        /// Returns the root folder that magic.lambda.io should treat as the root folder for its IO operations.
        /// </summary>
        string RootFolder { get; }
    }
}
