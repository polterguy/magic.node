/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Threading.Tasks;
using System.Collections.Generic;

namespace magic.node.contracts
{
    /// <summary>
    /// Common contract for handling files and folders in Magic.
    /// </summary>
    public interface IIOService
    {
        /// <summary>
        /// Returns true if file or folder exists.
        /// </summary>
        /// <param name="path">Absolute path to file</param>
        /// <returns>True if file exists</returns>
        bool Exists(string path);

        /// <summary>
        /// Returns true if file or folder exists.
        /// </summary>
        /// <param name="path">Absolute path to file</param>
        /// <returns>True if file exists</returns>
        Task<bool> ExistsAsync(string path);

        /// <summary>
        /// Deletes specified file or folder.
        /// </summary>
        /// <param name="path">Absolute path to file to delete</param>
        void Delete(string path);

        /// <summary>
        /// Deletes specified file or folder.
        /// </summary>
        /// <param name="path">Absolute path to file to delete</param>
        /// <returns>Awaitable task</returns>
        Task DeleteAsync(string path);

        /// <summary>
        /// Copies one file or folder to some destination.
        /// </summary>
        /// <param name="source">File to copy.</param>
        /// <param name="destination">Path to copy of file.</param>
        void Copy(string source, string destination);

        /// <summary>
        /// Copies one file or folder async to some destination.
        /// </summary>
        /// <param name="source">Path of file to copy</param>
        /// <param name="destination">Path to copy of file.</param>
        /// <returns>Awaitable task</returns>
        Task CopyAsync(string source, string destination);

        /// <summary>
        /// Moves a file or folder from source path to destination path.
        /// </summary>
        /// <param name="source">Path of file to move.</param>
        /// <param name="destination">New path for file.</param>
        void Move(string source, string destination);

        /// <summary>
        /// Moves a file or folder from source path to destination path async.
        /// </summary>
        /// <param name="source">Path of file to move.</param>
        /// <param name="destination">New path for file.</param>
        /// <returns>Awaitable task</returns>
        Task MoveAsync(string source, string destination);
    }
}
