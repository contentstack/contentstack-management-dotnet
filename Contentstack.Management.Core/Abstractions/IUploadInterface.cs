using System.Net.Http;

namespace Contentstack.Management.Core.Abstractions
{
    public interface IUploadInterface
    {
        string ContentType { get; set; }

        HttpContent GetHttpContent();
    }
}
