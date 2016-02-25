using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Core.Interfaces
{
    public interface IConfigurationProvider
    {
        T GetConfiguration<T>(Guid moduleId, T defaultConfiguration) where T : ConfigurationObjectBase;
        void UpdateConfiguration<T>(T configurationObject) where T : ConfigurationObjectBase;
    }
}
