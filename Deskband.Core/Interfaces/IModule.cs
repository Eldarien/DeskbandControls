using Deskband.Core.Configuration;
using Ninject;
using System;
using System.Drawing;

namespace Deskband.Core.Interfaces
{
    public interface IModule : IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        void Initialize(IKernel kernel);
        void ApplyConfiguration();
        void DoubleClick();
        void MouseWheel(int delta);
        void MousePoint(Point localPoint, Point globalPoint, Rectangle moduleScreenRectangle);
        void MousePointOut();
        ConfigurationObjectBase GetConfiguration();
    }
}
