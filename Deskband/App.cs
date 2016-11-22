using Deskband.Configuration;
using Deskband.Console;
using Deskband.Core.Common;
using Deskband.Core.Configuration;
using Deskband.Core.Interfaces;
using Deskband.Settings;
using Deskband.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband
{
    public class App
    {
        readonly Band _band;
        readonly IEnumerable<IModule> _modules;

        readonly ConfigurationProvider _config;
        readonly ConsoleHandler _consoleHandler;
        readonly IMenuProvider _menu;
        readonly ISizeProvider _sp;
        readonly ModuleContainer _moduleContainer;
        readonly FloatingForm _floatingForm;

        public App(
            Band band,
            IEnumerable<IModule> modules,
            ConfigurationProvider config,
            FloatingForm floatingForm,
            ConsoleHandler consoleHandler,
            IMenuProvider menu,
            ISizeProvider sp,
            ModuleContainer moduleContainer
            )
        {
            _band = band;
            _modules = modules;
            _config = config;
            _floatingForm = floatingForm;
            _consoleHandler = consoleHandler;
            _menu = menu;
            _sp = sp;
            _moduleContainer = moduleContainer;
        }

        public void Run()
        {
            _config.Load();

            _band.Close += OnClose;
            _moduleContainer.Resize += OnResize;

            ApplyConfiguration();

            //
            _band.TaskbarResized += (s, e) => ApplyConfiguration();

            // DPI
            _band.DPIChanged += (s, e) => ApplyConfiguration();

            // DoubleClick
            _band.MouseDoubleClick += (s, e) => HandleDoubleClick(e.Location);
            _floatingForm.MouseDoubleClick += (s, e) => HandleDoubleClick(e.Location);

            // Console
            var miConsole = _menu.AddItem(Guid.Empty, null, "Console", _consoleHandler.ToggleConsole);
            _consoleHandler.OnConsoleToggle += (s, e) => _menu.SetItemCheckedState(miConsole, e.Value);

            // Settings
            _menu.AddItem(Guid.Empty, null, "Settings", () =>
            {
                var sf = new SettingsForm(_config, _sp, _consoleHandler, _modules);
                sf.OnApply += (s, e) => { ApplyConfiguration(); _config.Save(); };
                sf.Show();
            });
        }

        private void OnClose(object sender, EventArgs e)
        {
            _config.Save();
        }

        private void OnResize(object sender, EventArgs e)
        {
            _consoleHandler.AddDebugLine(String.Format("Module container resized: {0}x{1}", _moduleContainer.Size.Width, _moduleContainer.Size.Height));

            if (!_floatingForm.Visible)
            {
                var tsi = _band.GetTaskbarSizeInfo();
                _band.MinSize = new Size(tsi.Mode == LayoutMode.Horizontal ? _moduleContainer.Size.Width : _moduleContainer.Size.Height, 0);
            }
            else
            {
                _band.MinSize = new Size(10, 0);
            }

            _band.ExecBandInfoChangedCommand();
            _floatingForm.Size = _moduleContainer.Size;
        }

        private void ApplyConfiguration()
        {
            var cfg = _config.GetConfiguration(Guid.Empty, ConfigurationModel.GetDefault());
            _config.UpdateConfiguration(cfg);

            _band.Controls.Clear();
            _floatingForm.Controls.Clear();

            if (cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband)
            {
                _floatingForm.Visible = false;
                _band.Controls.Add(_moduleContainer.AsControl());
            }
            else
            {
                _floatingForm.Visible = true;
                _floatingForm.Controls.Add(_moduleContainer.AsControl());
                _floatingForm.ApplyConfiguration();
            }

            var modulesSizeInfo = _modules
                .Select(x => new { Module = x, Configuration = x.GetConfiguration() as ConfigurationObjectBase })
                .OrderBy(x => x.Configuration.Order)
                .Select(m => new ModuleSizeInfo(m.Module.Id, m.Configuration.Disabled,
                    _sp.MakeSize(m.Configuration.Width, m.Configuration.Height), _sp.MakePoint(m.Configuration.Offset, 0)));

            var layoutMode = cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband ? _band.GetTaskbarSizeInfo().Mode : cfg.FloatingWindowSettings.Mode;
            _moduleContainer.PositionModules(modulesSizeInfo, cfg.GeneralSettings.DrawBorders, layoutMode);
            
            foreach (var m in _modules)
            {
                m.ApplyConfiguration();
            }

            OnResize(this, EventArgs.Empty);
        }

        private void HandleDoubleClick(Point location)
        {
            var moduleId = _moduleContainer.LocateModuleAtPoint(location);
            if (moduleId != null)
            {
                var module = _modules.First(m => m.Id == moduleId.Value);
                module.DoubleClick();
            }
        }
    }
}