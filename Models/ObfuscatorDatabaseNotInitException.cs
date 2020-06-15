using System;

namespace BadWordFilterApp.Models
{
    public class ObfuscatorDatabaseNotInitException : Exception
    {
        public ObfuscatorDatabaseNotInitException()
        {
        }

        public ObfuscatorDatabaseNotInitException(string message)
            : base(message)
        {
        }

        public ObfuscatorDatabaseNotInitException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
