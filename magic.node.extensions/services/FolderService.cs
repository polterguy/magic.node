/*
 * Magic Cloud, copyright Aista, Ltd and Thomas Hansen. See the attached LICENSE file for details. For license inquiries you can send an email to thomas@ainiro.io
 */

using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public Task CreateAsync(string path)
        {
            Create(path);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Delete(string path)
        {
            Directory.Delete(path, true);
        }

        /// <inheritdoc/>
        public Task DeleteAsync(string path)
        {
            Delete(path);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        /// <inheritdoc/>
        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(Exists(path));
        }

        /// <inheritdoc/>
        public void Move(string source, string destination)
        {
            Directory.Move(source, destination);
        }

        /// <inheritdoc/>
        public Task MoveAsync(string source, string destination)
        {
            Move(source, destination);
            return Task.CompletedTask;
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
        public Task CopyAsync(string source, string destination)
        {
            Copy(source, destination);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public List<string> ListFolders(string folder)
        {
            var result = Directory
                .GetDirectories(folder)
                .Select(x => x.Replace("\\", "/") + "/")
                .ToList();
            result.Sort();
            return result;
        }

        /// <inheritdoc/>
        public Task<List<string>> ListFoldersAsync(string folder)
        {
            return Task.FromResult(ListFolders(folder));
        }

        /// <inheritdoc/>
        public List<string> ListFoldersRecursively(string folder)
        {
            var tmpResult = Directory
                .GetDirectories(folder)
                .Select(x => x.Replace("\\", "/") + "/")
                .ToList();
            tmpResult.Sort();
            var result = new List<string>();
            foreach (var idx in tmpResult)
            {
                result.Add(idx);
                result.AddRange(ListFoldersRecursively(idx));
            }
            return result;
        }

        /// <inheritdoc/>
        public Task<List<string>> ListFoldersRecursivelyAsync(string folder)
        {
            return Task.FromResult(ListFoldersRecursively(folder));
        }
    }
}
