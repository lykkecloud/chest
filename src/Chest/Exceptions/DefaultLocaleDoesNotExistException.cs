using System;

namespace Chest.Exceptions
{
    public class DefaultLocaleDoesNotExistException : Exception
    {
        public DefaultLocaleDoesNotExistException()
        {
        }
        
        public DefaultLocaleDoesNotExistException(string message) : base(message)
        {
        }
    }
}