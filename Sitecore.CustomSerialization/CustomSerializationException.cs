namespace Sitecore.CustomSerialization
{
    using System;

    public class CustomSerializationException : Exception
    {
        public CustomSerializationException(string message) : base(message)
        {
        }

        public CustomSerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
