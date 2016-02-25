using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Core.Interfaces
{
    public interface IMenuProvider
    {
        string AddItem(string group, string name, Action handler);
        void AddSeparator(string group);
        void ClearGroup(string group);
        void SetItemEnabledState(string key, bool isEnabled);
        void SetItemCheckedState(string key, bool isChecked);
    }
}
