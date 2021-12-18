/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Threading.Tasks;
using System.Collections.Generic;

namespace magic.node.contracts
{
    /// <summary>
    /// Contract for handling files for magic.lambda.io
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Returns true if file exists.
        /// </summary>
        /// <param name="path">Absolute path to file</param>
        /// <returns>True if file exists</returns>
        bool Exists(string path);

        /// <summary>
        /// Returns true if file exists.
        /// </summary>
        /// <param name="path">Absolute path to file</param>
        /// <returns>True if file exists</returns>
        Task<bool> ExistsAsync(string path);

        /// <summary>
        /// Deletes specified file.
        /// </summary>
        /// <param name="path">Absolute path to file to delete</param>
        void Delete(string path);

        /// <summary>
        /// Deletes specified file.
        /// </summary>
        /// <param name="path">Absolute path to file to delete</param>
        /// <returns>Awaitable task</returns>
        Task DeleteAsync(string path);

        /// <summary>
        /// Copies one file to another destination.
        /// </summary>
        /// <param name="source">File to copy.</param>
        /// <param name="destination">Path to copy of file.</param>
        void Copy(string source, string destination);

        /// <summary>
        /// Copies file async to new path.
        /// </summary>
        /// <param name="source">Path of file to copy</param>
        /// <param name="destination">Path to copy of file.</param>
        /// <returns>Awaitable task</returns>
        Task CopyAsync(string source, string destination);

        /// <summary>
        /// Moves a file from source path to destination path.
        /// </summary>
        /// <param name="source">Path of file to move.</param>
        /// <param name="destination">New path for file.</param>
        void Move(string source, string destination);

        /// <summary>
        /// Moves a file from source path to destination path.
        /// </summary>
        /// <param name="source">Path of file to move.</param>
        /// <param name="destination">New path for file.</param>
        /// <returns>Awaitable task</returns>
        Task MoveAsync(string source, string destination);

        /// <summary>
        /// Loads a text file, and returns its content.
        /// </summary>
        /// <param name="path">Path of file to load.</param>
        /// <returns>Text content of file.</returns>
        string Load(string path);

        /// <summary>
        /// Loads a file async, and returns its content.
        /// </summary>
        /// <param name="path">Path of file to load.</param>
        /// <returns>Text content of file.</returns>
        Task<string> LoadAsync(string path);

        /// <summary>
        /// Saves the specified content into a file at the specified path.
        /// </summary>
        /// <param name="path">Path of file to save.</param>
        /// <param name="content">Content of file.</param>
        void Save(string path, string content);

        /// <summary>
        /// Saves the specified content into a file at the specified path async.
        /// </summary>
        /// <param name="path">Path of file to save.</param>
        /// <param name="content">Content of file.</param>
        /// <returns>Awaitable task</returns>
        Task SaveAsync(string path, string content);

        /// <summary>
        /// Saves the specified content into a file at the specified path.
        /// </summary>
        /// <param name="path">Path of file to save.</param>
        /// <param name="content">Content of file.</param>
        void Save(string path, byte[] content);

        /// <summary>
        /// Saves the specified content into a file at the specified path async.
        /// </summary>
        /// <param name="path">Path of file to save.</param>
        /// <param name="content">Content of file.</param>
        /// <returns>Awaitable task</returns>
        Task SaveAsync(string path, byte[] content);

        /// <summary>
        /// Returns all files found in the specified folder.
        /// </summary>
        /// <param name="folder">Folder to query for files.</param>
        /// <param name="extension">Optional extension files must have to be returned.</param>
        /// <returns>A list of absolute paths to all files found within specified folder.</returns>
        List<string> ListFiles(string folder, string extension = null);

        /// <summary>
        /// Returns all files found in the specified folder.
        /// </summary>
        /// <param name="folder">Folder to query for files.</param>
        /// <param name="extension">Optional extension files must have to be returned.</param>
        /// <returns>A list of absolute paths to all files found within specified folder.</returns>
        Task<List<string>> ListFilesAsync(string folder, string extension = null);
    }
}
