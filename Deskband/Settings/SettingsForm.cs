using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Deskband.Configuration;
using Deskband.Core.Configuration;
using Deskband.Core.Interfaces;
using Deskband.Console;

namespace Deskband.Settings
{
    public partial class SettingsForm : Form
    {
        private ConfigurationProvider _config;
        private ConsoleHandler _console;

        public event EventHandler OnApply;

        public SettingsForm(ConfigurationProvider config, ConsoleHandler console)
        {
            _config = config;
            _console = console;

            InitializeComponent();

            pgSettings.SelectedGridItemChanged += PgSettings_SelectedGridItemChanged;
            lbItems.SelectedIndexChanged += LbItems_SelectedIndexChanged;
            btnApply.Click += BtnApply_Click;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            foreach (var n in lbItems.Items) _config.UpdateConfiguration(n as ConfigurationObjectBase);
            OnApply?.Invoke(this, EventArgs.Empty);
        }

        private void PgSettings_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            var item = e.NewSelection.Value;
            //MessageBox.Show(item.ToString()); //TODO: display actions/controls supported by selected item

            //var it = item.GetType();
            //var ci = it.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConfigurationItemCommands<>)).ToList();
            //if (ci != null && ci.Any())
            //{
            //    var ca = ci.FirstOrDefault().GetGenericArguments()[0];
            //    _console.AddLine("item " + ca);
            //}
            //else if (it.IsGenericType && it.GetGenericTypeDefinition() == typeof(SettingsList<>))
            //{
            //    var li = it.BaseType.GetGenericArguments()[0];
            //    _console.AddLine("list " + li);
            //}

            var commands = e.NewSelection.PropertyDescriptor.Attributes.Cast<Attribute>().Where(x => x is SettingsCommandAttribute)
                .Union(e.NewSelection.Value.GetType().GetCustomAttributes(typeof(SettingsCommandAttribute), false));
            foreach (var c in commands.Cast<SettingsCommandAttribute>())
            {
                //c.ExecuteCommand(pgSettings.SelectedObject, item);
                _console.AddLine("attribute command, name = " + c.Name);
            }
        }

        private void LbItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            var obj = lbItems.Items[lbItems.SelectedIndex];
            pgSettings.SelectedObject = obj;
        }

        public void LoadDataAll(SettingsModel model)
        {
            lbItems.Items.Add(model.GlobalSettings);
            foreach (var ms in model.ModulesSettings)
            {
                lbItems.Items.Add(ms.SettingsObject);
            }
            lbItems.SelectedIndex = 0;
        }
    }
}