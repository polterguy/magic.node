/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Text;

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

        /// <summary>
        /// Returns the relative path given the absolute path as an argument.
        /// </summary>
        /// <param name="path">Absolute path of file or folder.</param>
        /// <returns>Relative file or folder path.</returns>
        string RelativePath(string path);

        /// <summary>
        /// Returns the absolute path given the relative path as an argument.
        /// </summary>
        /// <param name="path">Relative path of file or folder.</param>
        /// <returns>Absolute file or folder path.</returns>
        string AbsolutePath(string path);
    }
}
