/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.Threading.Tasks;
using System.Collections.Generic;

namespace magic.node.contracts
{
    /// <summary>
    /// Contract for handling folders in Magic.
    /// </summary>
    public interface IFolderService : IIOService
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
