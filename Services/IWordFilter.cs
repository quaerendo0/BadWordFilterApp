using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BadWordFilterApp.Services
{
    public interface IWordFilter
    {
        string FilterText(string text);
    }
}
