/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.IO;
using System.Threading.Tasks;
using magic.node.contracts;

namespace magic.node.services
{
    /// <inheritdoc/>
    public class StreamService : IStreamService
    {
        /// <inheritdoc/>
        public Stream OpenFile(string path)
        {
            return File.OpenRead(path);
        }

        /// <inheritdoc/>
        public void SaveFile(Stream stream, string path)
        {
            using (var fileStream = File.Create(path))
            {
                stream.CopyTo(fileStream);
            }
        }

        /// <inheritdoc/>
        public async Task SaveFileAsync(Stream stream, string path)
        {
            using (var fileStream = File.Create(path))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        /// <inheritdoc/>
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <inheritdoc/>
        public void Delete(string path)
        {
            File.Delete(path);
        }
    }
}
