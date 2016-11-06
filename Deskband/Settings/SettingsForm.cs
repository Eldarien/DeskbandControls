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
using System.Reflection;
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
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            foreach (var m in _settingsModel.SettingsModels.Cast<ConfigurationObjectBase>())
                _config.UpdateConfiguration(m);

            OnApply?.Invoke(this, EventArgs.Empty);

            BuildTreeView();
        }

        private void TvItems_AfterSelect(object sender, TreeViewEventArgs ea)
        {
            tsCommands.Items.Clear();
            var item = ea.Node.Tag;
            var prop = item as ModelDataWithPropertyInfo;
            if (prop == null)
            {
                var itemData = item as ModelDataWithParentData;
                if (itemData != null)
                {
                    var commands = itemData.Data.GetType().GetCustomAttributes(typeof(SettingsCommandAttribute), false);
                    foreach (var cmd in commands.Cast<SettingsCommandAttribute>())
                    {
                        _console.AddLine("attribute command, name = " + cmd.Name);

                        var tsItem = new ToolStripButton(cmd.Name);
                        tsItem.Click += (s, e) => { cmd.ExecuteCommand(itemData.ParentData, itemData.Data); BuildTreeView(); };
                        tsCommands.Items.Add(tsItem);
                    }
                    pgSettings.SelectedObject = itemData.Data;
                }
                else
                {
                    pgSettings.SelectedObject = item;
                }
            }
            else
            {
                var commands = prop.Info.GetCustomAttributes(typeof(SettingsCommandAttribute), false);
                foreach (var cmd in commands.Cast<SettingsCommandAttribute>())
                {
                    _console.AddLine("prop attribute command, name = " + cmd.Name);

                    var tsItem = new ToolStripButton(cmd.Name);
                    tsItem.Click += (s, e) => { cmd.ExecuteCommand(prop.Data, null); BuildTreeView(); };
                    tsCommands.Items.Add(tsItem);
                }

                pgSettings.SelectedObject = null;
            }
        }

        private void PgSettings_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            var item = e.NewSelection.Value;
            //TODO: display actions/controls supported by selected item

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

            //var commands = e.NewSelection.PropertyDescriptor
            //    .Attributes.Cast<Attribute>().Where(x => x is SettingsCommandAttribute)
            //    .Union(e.NewSelection.Value.GetType().GetCustomAttributes(typeof(SettingsCommandAttribute), false));
            //foreach (var c in commands.Cast<SettingsCommandAttribute>())
            //{
            //    //c.ExecuteCommand(pgSettings.SelectedObject, item);
            //    _console.AddLine("attribute command, name = " + c.Name);
            //}
        }

        private void BuildTreeView()
        {
            tvItems.Nodes.Clear();

            AddTreeNode(_settingsModel, "ROOT", null, true);

            // Remove root node
            var rootNode = tvItems.Nodes[0];
            tvItems.Nodes.Remove(rootNode);
            for (int i = rootNode.Nodes.Count - 1; i >= 0; i--)
            {
                var n = rootNode.Nodes[i];
                rootNode.Nodes.Remove(n);
                tvItems.Nodes.Insert(0, n);
            }

            tvItems.ExpandAll();

            if (pgSettings.SelectedObject != null)
            {
                var node = FindNode(tvItems.Nodes, pgSettings.SelectedObject);
                if (node != null)
                {
                    tvItems.SelectedNode = node;
                }
            }
        }

        private TreeNode FindNode(TreeNodeCollection nodes, object selectedObject)
        {
            foreach (TreeNode n in nodes)
            {
                var md = n.Tag as IModelData;
                if (md != null && md.Data == selectedObject) return n;

                if (n.Tag == selectedObject) return n;

                var nn = FindNode(n.Nodes, selectedObject);
                if (nn != null) return nn;
            }
            return null;
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

        private TreeNode AddTreeNode(object data, string text, TreeNode parentNode,
            bool isRootNode = false, bool doNotCheckForModuleNode = false, object parentData = null)
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

            var node = isRootNode ? null : new TreeNode { Text = text, Tag = parentData == null ? data : new ModelDataWithParentData(data, parentData) };
            if (!isRootNode)
            {
                if (parentNode == null)
                    tvItems.Nodes.Add(node);
                else
                    parentNode.Nodes.Add(node);
            }

            if (data != null)
            {
                var props = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
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
                            var listNode = AddTreeNode(new ModelDataWithPropertyInfo(data, prop), a.Name, node);
                            foreach (var item in list)
                            {
                                AddTreeNode(item, item.ToString(), listNode, parentData: data);
                            }
                        }
                    }
                }
            }
            return node;
        }
    }
}