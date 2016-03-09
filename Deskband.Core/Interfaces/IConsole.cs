using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Core.Interfaces
{
    public interface IConsole
    {
        void AddLine(string line);
        void AddDebugLine(string line);
    }
}
