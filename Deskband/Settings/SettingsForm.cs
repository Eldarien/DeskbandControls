using Deskband.Configuration;
using Deskband.Console;
using Deskband.Core.Configuration;
using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Deskband.Settings
{
    public partial class SettingsForm : Form
    {
        readonly ConfigurationProvider _config;
        readonly ConsoleHandler _console;
        readonly IEnumerable<IModule> _modules;

        public event EventHandler OnApply;

        private SettingsModel _settingsModel;

        public SettingsForm(ConfigurationProvider config, ConsoleHandler console, IEnumerable<IModule> modules)
        {
            _config = config;
            _console = console;
            _modules = modules;
            //_settingsModel = settingsModel;

            InitializeComponent();

            LoadProfilesList();

            cbProfiles.SelectedIndexChanged += CbProfiles_SelectedIndexChanged;
            btnSave.Click += BtnSave_Click;
            btnLoad.Click += BtnLoad_Click;
            btnOK.Click += BtnOK_Click;
            btnApply.Click += BtnApply_Click;
            btnCancel.Click += BtnCancel_Click;
            tvItems.AfterSelect += TvItems_AfterSelect;

            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            var cfg = _config.GetConfiguration(Guid.Empty, ConfigurationModel.GetDefault());
            var sm = new SettingsModel { SettingsModels = new List<ConfigurationObjectBase> { cfg } };
            sm.SettingsModels.AddRange(_modules.Select(x => x.GetConfiguration()).OrderBy(x => x.Order));
            _settingsModel = sm;

            BuildTreeView();
        }

        private void LoadProfilesList()
        {
            cbProfiles.Items.Clear();
            cbProfiles.Items.Add("< New profile >");
            cbProfiles.SelectedIndex = 0;
            foreach (var p in _config.GetProfiles()) cbProfiles.Items.Add(p);

            CbProfiles_SelectedIndexChanged(cbProfiles, EventArgs.Empty);
        }

        private void CbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            var isNew = cbProfiles.SelectedIndex == 0;
            btnLoad.Enabled = !isNew;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var profileName = cbProfiles.SelectedItem.ToString();
            if (profileName.StartsWith("<"))
            {
                profileName = Microsoft.VisualBasic.Interaction.InputBox("Enter new profile name:", " ", "", -1, -1);
            }
            if (String.IsNullOrWhiteSpace(profileName))
                return;

            if (_config.ProfileExists(profileName))
            {
                var q = MessageBox.Show($"Profile \"{profileName}\" already exists. Overwrite?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (q != DialogResult.Yes)
                    return;
            }

            BtnApply_Click(sender, e);
            _config.Save(profileName);
            LoadProfilesList();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            var profileName = cbProfiles.SelectedItem.ToString();
            _config.Load(profileName);

            LoadConfiguration();
            BtnApply_Click(sender, e);

            tvItems.SelectedNode = tvItems.Nodes[0];
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            BtnApply_Click(sender, e);
            Close();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            foreach (var m in _settingsModel.SettingsModels.Cast<ConfigurationObjectBase>())
                _config.UpdateConfiguration(m);

            OnApply?.Invoke(this, EventArgs.Empty);

            var obj = pgSettings.SelectedObject;
            BuildTreeView();
            SelectNodeForObject(obj);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TvItems_AfterSelect(object sender, TreeViewEventArgs ea)
        {
            var nodeData = ea.Node.Tag as NodeData;
            if (nodeData == null)
            {
                pgSettings.SelectedObject = ea.Node.Tag;
                BindCommands(null, null, null);
            }
            else if (nodeData.DataType == NodeDataType.Item)
            {
                pgSettings.SelectedObject = nodeData.ItemData;
                BindCommands(nodeData.ItemData.GetType(), nodeData.ParentData, nodeData.ItemData);
            }
            else if (nodeData.DataType == NodeDataType.List)
            {
                pgSettings.SelectedObject = null;
                BindCommands(nodeData.ListPropertyInfo, nodeData.ItemData, null);
            }

            ShowStubIfNoSettings();
        }

        private void BindCommands(MemberInfo memberInfo, object cmdInstance, object cmdArgument)
        {
            tsCommands.Items.Clear();
            if (memberInfo == null) return;

            var commands = memberInfo.GetCustomAttributes(typeof(SettingsCommandAttribute), true);
            foreach (var cmd in commands.Cast<SettingsCommandAttribute>())
            {
                if (cmd.IsAvailable(cmdInstance, cmdArgument))
                {
                    var tsItem = new ToolStripButton(cmd.Name);
                    tsItem.Click += (s, e) =>
                    {
                        var r = cmd.ExecuteCommand(cmdInstance, cmdArgument);
                        BuildTreeView();
                        SelectNodeForObject(r);
                    };
                    tsCommands.Items.Add(tsItem);
                }
            }
        }

        private void ShowStubIfNoSettings()
        {
            var view = pgSettings.GetType().GetField("gridView", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(pgSettings);
            var pgItemList = (GridItemCollection)view.GetType()
                .InvokeMember("GetAllGridEntries", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, view, null);
            if (pgItemList == null)
            {
                pgSettings.SelectedObject = new { Info = "Please select a subpage" };
            }
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
        }

        private void SelectNodeForObject(object obj)
        {
            if (obj != null)
            {
                var node = FindNode(tvItems.Nodes, obj);
                if (node != null)
                {
                    tvItems.SelectedNode = node;
                }
            }
        }

        private TreeNode FindNode(TreeNodeCollection nodes, object obj)
        {
            foreach (TreeNode n in nodes)
            {
                if (n.Tag == obj) return n;

                var nodeData = n.Tag as NodeData;
                if (nodeData != null)
                {
                    if (nodeData.ItemData == obj) return n;
                    if (nodeData.DataType == NodeDataType.List
                        && nodeData.ListPropertyInfo.GetValue(nodeData.ItemData, null) == obj) return n;
                }

                var nn = FindNode(n.Nodes, obj);
                if (nn != null) return nn;
            }
            return null;
        }

        private TreeNode GetTreeNodeByModuleId(TreeNodeCollection nodes, Guid id)
        {
            foreach (TreeNode node in nodes)
            {
                var ms = node.Tag as ModuleSettings;
                if (ms == null) continue;
                if (ms.Id != id) continue;
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

            var node = isRootNode ? null : new TreeNode { Text = text, Tag = parentData == null ? data : new NodeData(data, parentData) };
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
                            var nodeData = new NodeData(data, prop);
                            var listNode = AddTreeNode(nodeData, a.Name, node);
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