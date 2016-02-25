using Ninject;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Deskband.Core.Interfaces
{
    public interface IModule : IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        void Initialize(IKernel kernel);
        void ApplyConfiguration();
    }
}
