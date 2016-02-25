using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Core.Interfaces
{
    public interface IModuleContainer
    {
        void AddControl(Guid moduleId, Control control, bool isLastChild = false);
        void ClearControls(Guid moduleId);

        Size GetSize(Guid moduleId);

        void Show(Guid moduleId);
        void Hide(Guid moduleId);
    }
}
