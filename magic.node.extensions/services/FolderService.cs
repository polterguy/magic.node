/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.IO;
using System.Collections.Generic;
using magic.node.extensions;
using magic.node.contracts;

namespace magic.node.services
{
    /// <inheritdoc/>
    public class FolderService : IFolderService
    {
        /// <inheritdoc/>
        public void Create(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <inheritdoc/>
        public void Delete(string path)
        {
            Directory.Delete(path, true);
        }

        /// <inheritdoc/>
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc/>
        public void Move(string source, string destination)
        {
            Directory.Move(source, destination);
        }

        /// <inheritdoc/>
        public void Copy(string source, string destination)
        {
            // Sanity checking invocation, and verifying source folder exists.
            var sourceFolder = new DirectoryInfo(source);
            if (!sourceFolder.Exists)
                throw new HyperlambdaException($"Source directory does not exist or could not be found '{source}'");
            Directory.CreateDirectory(destination);

            foreach (var file in sourceFolder.GetFiles())
            {
                file.CopyTo(Path.Combine(destination, file.Name), false);
            }
            foreach (var idxSub in sourceFolder.GetDirectories())
            {
                Copy(idxSub.FullName, Path.Combine(destination, idxSub.Name));
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> ListFolders(string folder)
        {
            return Directory.GetDirectories(folder);
        }
    }
}
