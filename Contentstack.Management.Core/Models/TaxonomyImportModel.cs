using System;
using System.IO;
using System.Net.Http;
using Contentstack.Management.Core.Abstractions;

namespace Contentstack.Management.Core.Models
{
    /// <summary>
    /// Model for Taxonomy import (file upload). Implements IUploadInterface for multipart form with key "taxonomy".
    /// </summary>
    public class TaxonomyImportModel : IUploadInterface
    {
        private readonly Stream _fileStream;
        private readonly string _fileName;

        public string ContentType { get; set; } = "multipart/form-data";

        /// <summary>
        /// Creates an import model from a file path.
        /// </summary>
        public TaxonomyImportModel(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));
            _fileName = Path.GetFileName(filePath);
            _fileStream = File.OpenRead(filePath);
        }

        /// <summary>
        /// Creates an import model from a stream (e.g. JSON or CSV taxonomy file).
        /// </summary>
        /// <param name="stream">Stream containing taxonomy file content.</param>
        /// <param name="fileName">Name to use for the form part (e.g. "taxonomy.json").</param>
        public TaxonomyImportModel(Stream stream, string fileName = "taxonomy.json")
        {
            _fileStream = stream ?? throw new ArgumentNullException(nameof(stream));
            _fileName = fileName ?? "taxonomy.json";
        }

        public HttpContent GetHttpContent()
        {
            var streamContent = new StreamContent(_fileStream);
            var content = new MultipartFormDataContent();
            content.Add(streamContent, "taxonomy", _fileName);
            return content;
        }
    }
}
