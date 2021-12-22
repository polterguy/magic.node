﻿/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.IO;
using System.Threading.Tasks;

namespace magic.node.contracts
{
    /// <summary>
    /// Contract for handling streams for magic.lambda.io.
    /// </summary>
    public interface IStreamService
    {
        /// <summary>
        /// Returns a stream wrapping the specified filename.
        /// </summary>
        /// <param name="path">Absolute path to file</param>
        /// <returns>Open Stream object</returns>
        Stream OpenFile(string path);

        /// <summary>
        /// Returns a stream wrapping the specified filename.
        /// </summary>
        /// <param name="path">Absolute path to file</param>
        /// <returns>Open Stream object</returns>
        Task<Stream> OpenFileAsync(string path);

        /// <summary>
        /// Saves the specified stream to the specified filename.
        /// </summary>
        /// <param name="stream">Stream to save content of</param>
        /// <param name="path">Absolute path to filename to save stream's content to</param>
        void SaveFile(Stream stream, string path);

        /// <summary>
        /// Saves the specified stream to the specified filename.
        /// </summary>
        /// <param name="stream">Stream to save content of</param>
        /// <param name="path">Absolute path to filename to save stream's content to</param>
        /// <returns>Awaitable task</returns>
        Task SaveFileAsync(Stream stream, string path);
    }
}
