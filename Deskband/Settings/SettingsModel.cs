using Deskband.Configuration;
using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Settings
{
    public class SettingsModel
    {
        [SettingsNodeList("SettingsModels")]
        public List<ConfigurationObjectBase> SettingsModels { get; set; }

        public static bool IsMoveAvailable(SettingsModel model, ConfigurationObjectBase args)
        {
            return args.ModuleId != Guid.Empty;
        }

        public static object MoveUp(SettingsModel model, ConfigurationObjectBase args)
        {
            var index = model.SettingsModels.IndexOf(args);
            if (index > 0)
            {
                model.SettingsModels.Remove(args);
                model.SettingsModels.Insert(index - 1, args);
            }
            UpdateOrders(model);
            return args;
        }

        public static object MoveDown(SettingsModel model, ConfigurationObjectBase args)
        {
            var index = model.SettingsModels.IndexOf(args);
            if (index < model.SettingsModels.Count - 1)
            {
                model.SettingsModels.Remove(args);
                model.SettingsModels.Insert(index + 1, args);
            }
            UpdateOrders(model);
            return args;
        }

        private static void UpdateOrders(SettingsModel model)
        {
            for (int i = 0; i <= model.SettingsModels.Count - 1; i++)
            {
                model.SettingsModels[i].Order = i;
            }
        }
    }
}