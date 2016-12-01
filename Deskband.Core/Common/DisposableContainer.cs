using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Core.Common
{
    public class DisposableContainer : IDisposable
    {
        private List<IDisposable> _objects = new List<IDisposable>();

        public T Add<T>(T obj) where T : IDisposable
        {
            _objects.Add(obj);
            return obj;
        }

        public void Dispose()
        {
            if (_objects != null)
            {
                for (int i = _objects.Count - 1; i >= 0; i--)
                {
                    _objects[i].Dispose();
                }
                _objects = null;
            }
        }
    }
}
