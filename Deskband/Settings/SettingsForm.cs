using Deskband.Configuration;
using Deskband.Console;
using Deskband.Core.Configuration;
using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Settings
{
    public partial class SettingsForm : Form
    {
        private ConfigurationProvider _config;
        private ConsoleHandler _console;

        public event EventHandler OnApply;

        private SettingsModel _settingsModel;

        public SettingsForm(ConfigurationProvider config, ConsoleHandler console, SettingsModel settingsModel)
        {
            _config = config;
            _console = console;

            _settingsModel = settingsModel;

            InitializeComponent();

            btnApply.Click += BtnApply_Click;
            tvItems.AfterSelect += TvItems_AfterSelect;
            pgSettings.SelectedGridItemChanged += PgSettings_SelectedGridItemChanged;

            BuildTreeView();
            tvItems.ExpandAll();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            //_config.UpdateConfiguration(_settingsModel.GlobalSettings);
            //foreach (var m in _settingsModel.ModulesSettings.Cast<ConfigurationObjectBase>())
            //    _config.UpdateConfiguration(m);

            OnApply?.Invoke(this, EventArgs.Empty);
        }

        private void TvItems_AfterSelect(object sender, TreeViewEventArgs e)
        {
            pgSettings.SelectedObject = e.Node.Tag;
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

        private void BuildTreeView()
        {
            //var root = _settingsModel.SettingsModels.Cast<ConfigurationObjectBase>().FirstOrDefault(x => x.ModuleId == Guid.Empty);
            //var others = _settingsModel.SettingsModels.Cast<ConfigurationObjectBase>().Except(new[] { root });
            AddTreeNode(_settingsModel, "ROOT", null, true);
        }

        private TreeNode GetTreeNodeByModuleId(TreeNodeCollection nodes, Guid id)
        {
            foreach (TreeNode node in nodes)
            {
                _console.AddDebugLine("Cheking node " + node.Text);
                var ms = node.Tag as ModuleSettings;
                if (ms == null) continue;
                _console.AddDebugLine("Found module settings " + ms.Id);
                if (ms.Id != id) continue;
                _console.AddDebugLine("Found It!");
                return node;
            }
            foreach (TreeNode node in nodes)
            {
                var result = GetTreeNodeByModuleId(node.Nodes, id);
                if (result != null) return result;
            }
            return null;
        }

        private TreeNode AddTreeNode(object data, string text, TreeNode parentNode, bool isRootNode = false, bool doNotCheckForModuleNode = false)
        {
            // Search for node with specified Id, if found - use it as parent node
            var confModelData = data as ConfigurationObjectBase;
            if (!doNotCheckForModuleNode && confModelData != null && confModelData.ModuleId != Guid.Empty)
            {
                var moduleNode = GetTreeNodeByModuleId(tvItems.Nodes, confModelData.ModuleId);
                if (moduleNode != null)
                {
                    return AddTreeNode(data, text, moduleNode, isRootNode, true);
                }
            }

            var node = isRootNode ? null : new TreeNode { Text = text, Tag = data };
            if (!isRootNode)
            {
                if (parentNode == null)
                    tvItems.Nodes.Add(node);
                else
                    parentNode.Nodes.Add(node);
            }

            if (data != null)
            {
                var props = data.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var prop in props)
                {
                    foreach (var a in prop.GetCustomAttributes(typeof(SettingsNodeAttribute), false).Cast<SettingsNodeAttribute>())
                    {
                        var propData = prop.GetValue(data, null);
                        AddTreeNode(propData, a.Name, node);
                    }

                    foreach (var a in prop.GetCustomAttributes(typeof(SettingsNodeListAttribute), false).Cast<SettingsNodeListAttribute>())
                    {
                        var list = prop.GetValue(data, null) as IEnumerable<object>;
                        if (list != null)
                        {
                            var listNode = AddTreeNode(null, a.Name, node);
                            foreach (var item in list)
                            {
                                AddTreeNode(item, item.ToString(), listNode);
                            }
                        }
                    }
                }
            }
            return node;
        }
    }
}