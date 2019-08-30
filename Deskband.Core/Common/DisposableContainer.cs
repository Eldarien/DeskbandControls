using System;
using System.Collections.Generic;
using System.Linq;

namespace Deskband.Core.Common
{
    public class DisposableContainer : IDisposable
    {
        private readonly object _locker = new object();
        private List<IDisposable> _objects = new List<IDisposable>();

        public T Add<T>(T obj) where T : IDisposable
        {
            lock (_locker)
            {
                _objects.Add(obj);
            }
            return obj;
        }

        /// <summary>
        /// Disposes each object in the list and destroys the list
        /// </summary>
        public void Dispose()
        {
            lock (_locker)
            {
                DisposeAllItems();
                _objects = null;
            }
        }

        /// <summary>
        /// Disposes each object in the list and clears the list.
        /// </summary>
        public void DisposeRemoveItems()
        {
            lock (_locker)
            {
                DisposeAllItems();
                _objects.Clear();
            }
        }

        /// <summary>
        /// Disposes specified item and removes it from list
        /// </summary>
        public void DisposeRemoveItem<T>(T obj) where T : IDisposable
        {
            lock (_locker)
            {
                _objects.Remove(obj);
                obj.Dispose();
            }
        }

        private void DisposeAllItems()
        {
            if (_objects != null && _objects.Any())
            {
                for (int i = _objects.Count - 1; i >= 0; i--)
                {
                    _objects[i].Dispose();
                }
            }
        }
    }
}
