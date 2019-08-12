using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anthill.Common.AzureBlob
{
    public class AzureBlobClient
    {
        private CloudBlobClient _blobClient;

        public AzureBlobClient(String connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="containerName">Name of the container. (should contains only lower case letters a-z)</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileContent">Content of the file.</param>
        /// <returns></returns>
        public async Task UploadFile(String containerName, String fileName, byte[] fileContent)
        {
            var block = await GetFileBlock(containerName, fileName);
            await block.UploadFromByteArrayAsync(fileContent, 0, fileContent.Length);
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="containerName">Name of the container. (should contains only lower case letters a-z)</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileContent">Content of the file.</param>
        /// <returns></returns>
        public async Task<string> UploadFile(String containerName, String fileName, Stream fileContent)
        {
            var block = await GetFileBlock(containerName, fileName);
            await block.UploadFromStreamAsync(fileContent);
            return block.Uri.ToString();
        }


        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="containerName">Name of the container. (should contains only lower case letters a-z)</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public async Task<byte[]> DownloadFile(String containerName, String fileName)
        {
            var block = await GetFileBlock(containerName, fileName);

            using (var stream = new MemoryStream())
            {
                await block.DownloadToStreamAsync(stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="containerName">Name of the container. (should contains only lower case letters a-z)</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public async Task<Stream> DownloadFileStream(String containerName, String fileName)
        {
            var block = await GetFileBlock(containerName, fileName);
            var stream = new MemoryStream();
            await block.DownloadToStreamAsync(stream);
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="containerName">Name of the container. (should contains only lower case letters a-z)</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public async Task DeleteFile(String containerName, String fileName)
        {
            var block = await GetFileBlock(containerName, fileName);
            await block.DeleteAsync();
        }

        /// <summary>
        /// Gets the objects URI.
        /// </summary>
        /// <param name="containerName">Name of the container. (should contains only lower case letters a-z)</param>
        /// <returns></returns>
        public async Task<IEnumerable<Uri>> GetObjectsUri(String containerName)
        {
            var container = _blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            BlobContinuationToken continuationToken = null;

            var result = new List<Uri>();

            do
            {
                var resultSegment = await container.ListBlobsSegmentedAsync(continuationToken);
                result.AddRange(resultSegment.Results.Select(x => x.Uri));
            }
            while (continuationToken != null);

            return result;
        }

        /// <summary>
        /// Deletes the container.
        /// </summary>
        /// <param name="containerName">Name of the container. (should contains only lower case letters a-z)</param>
        /// <returns></returns>
        public async Task DeleteContainer(String containerName)
        {
            var container = _blobClient.GetContainerReference(containerName);
            await container.DeleteAsync();
        }

        private async Task<CloudBlockBlob> GetFileBlock(String containerName, String fileName)
        {
            var container = _blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            return container.GetBlockBlobReference(fileName);
        }
    }
}
