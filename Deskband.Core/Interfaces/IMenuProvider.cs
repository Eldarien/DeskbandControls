using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Core.Interfaces
{
    public interface IMenuProvider
    {
        //string AddItem(Guid moduleId, string group, string name, Action handler);
        //void AddSeparator(Guid moduleId, string group);
        //void ClearGroup(string group);
        //void SetItemEnabledState(string key, bool isEnabled);
        //void SetItemCheckedState(string key, bool isChecked);

        Guid AddItem(Guid moduleId, Guid? parentId, string text, Action handler);
        void RemoveItem(Guid id);
        void ClearByModule(Guid moduleId);
        void SetItemEnabledState(Guid id, bool isEnabled);
        void SetItemCheckedState(Guid id, bool isChecked);
    }
}
