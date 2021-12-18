/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Threading.Tasks;
using System.Collections.Generic;

namespace magic.node.contracts
{
    /// <summary>
    /// Contract for handling folders for magic.lambda.io
    /// </summary>
    public interface IFolderService
    {
        /// <summary>
        /// Creates a new folder with hte specified path.
        /// </summary>
        /// <param name="path">Path to folder.</param>
        void Create(string path);

        /// <summary>
        /// Creates a new folder with hte specified path.
        /// </summary>
        /// <param name="path">Path to folder.</param>
        /// <returns>Awaitable task</returns>
        Task CreateAsync(string path);

        /// <summary>
        /// returns true if specified folder exists.
        /// </summary>
        /// <param name="path">Path to folder to check for.</param>
        /// <returns>True if folder exists.</returns>
        bool Exists(string path);

        /// <summary>
        /// returns true if specified folder exists.
        /// </summary>
        /// <param name="path">Path to folder to check for.</param>
        /// <returns>True if folder exists.</returns>
        Task<bool> ExistsAsync(string path);

        /// <summary>
        /// Deletes the specified folder.
        /// </summary>
        /// <param name="path">Path of folder to delete.</param>
        void Delete(string path);

        /// <summary>
        /// Deletes the specified folder.
        /// </summary>
        /// <param name="path">Path of folder to delete.</param>
        /// <returns>Awaitable task</returns>
        Task DeleteAsync(string path);

        /// <summary>
        /// Moves the specified folder.
        /// </summary>
        /// <param name="source">Path of folder to move.</param>
        /// <param name="destination">New path of folder.</param>
        void Move(string source, string destination);

        /// <summary>
        /// Moves the specified folder.
        /// </summary>
        /// <param name="source">Path of folder to move.</param>
        /// <param name="destination">New path of folder.</param>
        /// <returns>Awaitable task</returns>
        Task MoveAsync(string source, string destination);

        /// <summary>
        /// Copies the specified folder.
        /// </summary>
        /// <param name="source">Path of folder to copy.</param>
        /// <param name="destination">Destination path of where to copy folder.</param>
        void Copy(string source, string destination);

        /// <summary>
        /// Copies the specified folder.
        /// </summary>
        /// <param name="source">Path of folder to copy.</param>
        /// <param name="destination">Destination path of where to copy folder.</param>
        /// <returns>Awaitable task</returns>
        Task CopyAsync(string source, string destination);

        /// <summary>
        /// Lists all folders within the specified folder.
        /// </summary>
        /// <param name="folder">Folder to query for folders.</param>
        /// <returns>Absolute paths to list of folders the specified folder contains</returns>
        List<string> ListFolders(string folder);

        /// <summary>
        /// Lists all folders within the specified folder.
        /// </summary>
        /// <param name="folder">Folder to query for folders.</param>
        /// <returns>Absolute paths to list of folders the specified folder contains</returns>
        Task<List<string>> ListFoldersAsync(string folder);
    }
}
