using System;

namespace Retroactiune.Core.Services
{
    public class GenericServiceException : Exception
    {
        public GenericServiceException(string message) : base(message)
        {
        }
    }
}