using System;
namespace Contentstack.Management.Core.Exceptions
{
    public class ContentstackException : Exception
    {
        public ContentstackException(string message) : base(message) { }

        public ContentstackException(string message, Exception innerException) : base(message, innerException) { }
    }
}
