using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BadWordFilterApp.Services
{
    public class FilterInitializationException : Exception
    {
        public FilterInitializationException()
        {
        }

        public FilterInitializationException(string message)
            : base(message)
        {
        }

        public FilterInitializationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
