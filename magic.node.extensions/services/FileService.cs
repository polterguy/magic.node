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
        public List<string> ListFilesRecursively(string folder, string extension = null)
        {
            var tmpResult = string.IsNullOrEmpty(extension) ?
                Directory
                    .GetFiles(folder)
                    .Select(x => x.Replace("\\", "/"))
                    .ToList() :
                Directory
                    .GetFiles(folder)
                    .Where(x => Path.GetExtension(x) == extension)
                    .Select(x => x.Replace("\\", "/"))
                    .ToList();
            tmpResult.Sort();
            var result = new List<string>();
            foreach (var idx in tmpResult)
            {
                result.Add(idx);
            }
            foreach (var idx in Directory.GetDirectories(folder))
            {
                result.AddRange(ListFilesRecursively(idx, extension));
            }
            return result;
        }

        /// <inheritdoc/>
        public Task<List<string>> ListFilesRecursivelyAsync(string folder, string extension = null)
        {
            return Task.FromResult(ListFilesRecursively(folder, extension));
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
        public IEnumerable<(string Filename, string Content)> LoadRecursively(
            string folder,
            string extension)
        {
            return LoadRecursivelyAsync(folder, extension).GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<(string Filename, string Content)>> LoadRecursivelyAsync(
            string folder,
            string extension)
        {
            var files = await ListFilesRecursivelyAsync(folder, extension);
            var result = new List<(string Filename, string Content)>();
            foreach (var idx in files)
            {
                result.Add((idx, await LoadAsync(idx)));
            }
            return result;
        }

        /// <inheritdoc/>
        public byte[] LoadBinary(string path)
        {
            return File.ReadAllBytes(path);
        }

        /// <inheritdoc/>
        public Task<byte[]> LoadBinaryAsync(string path)
        {
            return Task.FromResult(File.ReadAllBytes(path));
        }

        /// <inheritdoc/>
        public void Save(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        /// <inheritdoc/>
        public void Save(string path, byte[] content)
        {
            File.WriteAllBytes(path, content);
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
        public async Task SaveAsync(string path, byte[] content)
        {
            using (var writer = File.Create(path))
            {
                await writer.WriteAsync(content, 0, content.Length);
            }
        }
    }
}
