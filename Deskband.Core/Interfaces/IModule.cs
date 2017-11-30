using Deskband.Core.Configuration;
using Ninject;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Deskband.Core.Interfaces
{
    public interface IModule : IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        void Initialize(IKernel kernel);
        void ApplyConfiguration();
        void MouseDoubleClick(MouseButtons button);
        void MouseClick(MouseButtons button);
        void MouseWheel(int delta);
        void MousePoint(Point localPoint, Point globalPoint, Rectangle moduleScreenRectangle);
        void MousePointOut();
        ConfigurationObjectBase GetConfiguration();
    }
}
