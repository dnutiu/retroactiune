using System;

namespace Retroactiune.Services
{
    public class GenericServiceException : Exception
    {
        public GenericServiceException(string message) : base(message)
        {
        }
    }
}