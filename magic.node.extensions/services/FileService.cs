/*
 * Magic Cloud, copyright Aista, Ltd. See the attached LICENSE file for details.
 */

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using magic.node.contracts;
using System.Collections.Generic;

namespace magic.node.services
{
    /// <inheritdoc/>
    public class FileService : IFileService
    {
        /// <inheritdoc/>
        public void Copy(string source, string destination)
        {
            File.Copy(source, destination);
        }

        /// <inheritdoc/>
        public async Task CopyAsync(string source, string destination)
        {
            using (Stream sourceStream = File.OpenRead(source))
            {
                using (Stream destinationStream = File.Create(destination))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
        }

        /// <inheritdoc/>
        public void Move(string source, string destination)
        {
            File.Move(source, destination);
        }

        /// <inheritdoc/>
        public Task MoveAsync(string source, string destination)
        {
            Move(source, destination);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Delete(string path)
        {
            File.Delete(path);
        }

        /// <inheritdoc/>
        public Task DeleteAsync(string path)
        {
            File.Delete(path);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <inheritdoc/>
        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(Exists(path));
        }

        /// <inheritdoc/>
        public List<string> ListFiles(string folder, string extension = null)
        {
            var files = string.IsNullOrEmpty(extension) ?
                Directory
                    .GetFiles(folder)
                    .Select(x => x.Replace("\\", "/"))
                    .ToList() :
                Directory
                    .GetFiles(folder)
                    .Where(x => Path.GetExtension(x) == extension)
                    .Select(x => x.Replace("\\", "/"))
                    .ToList();
            files.Sort();
            return files;
        }

        /// <inheritdoc/>
        public Task<List<string>> ListFilesAsync(string folder, string extension = null)
        {
            return Task.FromResult(ListFiles(folder, extension));
        }

        /// <inheritdoc/>
        public string Load(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }

        /// <inheritdoc/>
        public async Task<string> LoadAsync(string path)
        {
            using (var file = File.OpenText(path))
            {
                return await file.ReadToEndAsync();
            }
        }

        /// <inheritdoc/>
        public void Save(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        /// <inheritdoc/>
        public async Task SaveAsync(string path, string content)
        {
            using (var writer = File.CreateText(path))
            {
                await writer.WriteAsync(content);
            }
        }

        /// <inheritdoc/>
        public void Save(string path, byte[] content)
        {
            File.WriteAllBytes(path, content);
        }

        /// <inheritdoc/>
        public async Task SaveAsync(string path, byte[] content)
        {
            using (var writer = File.Create(path))
            {
                await writer.WriteAsync(content, 0, content.Length);
            }
        }
    }
}
