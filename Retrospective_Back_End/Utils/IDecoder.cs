using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Retrospective_Back_End.Utils
{
    public interface IDecoder
    {
        string DecodeToken(string token);
    }
}
