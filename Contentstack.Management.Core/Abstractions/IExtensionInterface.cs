namespace Contentstack.Management.Core.Abstractions
{
    public interface IExtensionInterface : IUploadInterface
    {
        string Title { get; set; }
        string Tags { get; set; }
    }
}
