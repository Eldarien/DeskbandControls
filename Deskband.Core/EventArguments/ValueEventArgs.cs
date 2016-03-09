using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Core.EventArguments
{
    public class ValueEventArgs<T> : EventArgs
    {
        public T Value { get; private set; }

        public ValueEventArgs(T value)
        {
            Value = value;
        }
    }
}
