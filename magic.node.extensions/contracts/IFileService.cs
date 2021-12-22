/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Threading.Tasks;
using System.Collections.Generic;

namespace magic.node.contracts
{
    /// <summary>
    /// Contract for handling files in Magic.
    /// </summary>
    public interface IFileService : IIOService
    {
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
        /// Loads a binary file, and returns its content.
        /// </summary>
        /// <param name="path">Path of file to load.</param>
        /// <returns>Binary content of file.</returns>
        byte[] LoadBinary(string path);

        /// <summary>
        /// Loads a binary file async, and returns its content.
        /// </summary>
        /// <param name="path">Path of file to load.</param>
        /// <returns>Binary content of file.</returns>
        Task<byte[]> LoadBinaryAsync(string path);

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
