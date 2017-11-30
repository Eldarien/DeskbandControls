using Deskband.Configuration;
using Deskband.Console;
using Deskband.Core.Common;
using Deskband.Core.Configuration;
using Deskband.Core.Interfaces;
using Deskband.Core.WinApi;
using Deskband.Integration;
using Deskband.Settings;
using Deskband.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Deskband
{
    public class App
    {
        private const int BandMinWidth = 10; // Reserved width to be able to click on to access context menu

        readonly Band _band;
        readonly IEnumerable<IModule> _modules;

        readonly ConfigurationProvider _config;
        readonly ConsoleHandler _console;
        readonly IMenuProvider _menu;
        readonly ISizeProvider _sp;
        readonly ModuleContainer _moduleContainer;
        readonly FloatingForm _floatingForm;
        readonly TooltipProvider _tooltipProvider;

        private SettingsForm _settingsForm;

        public App(
            Band band,
            IEnumerable<IModule> modules,
            ConfigurationProvider config,
            FloatingForm floatingForm,
            ConsoleHandler consoleHandler,
            IMenuProvider menu,
            ISizeProvider sp,
            ModuleContainer moduleContainer,
            TooltipProvider tooltipProvider
            )
        {
            _band = band;
            _modules = modules;
            _config = config;
            _floatingForm = floatingForm;
            _console = consoleHandler;
            _menu = menu;
            _sp = sp;
            _moduleContainer = moduleContainer;
            _tooltipProvider = tooltipProvider;
        }

        public void Run()
        {
            _config.Load();

            _band.Close += OnClose;
            _moduleContainer.Resize += OnResize;

            ApplyConfiguration();

            _config.ConfigurationFileChanged += OnConfigurationFileChanged;
            _band.TaskbarResized += (s, e) => ApplyConfiguration();
            _band.DPIChanged += (s, e) => ApplyConfiguration();
            _band.MouseDoubleClick += (s, e) => HandleDoubleClick(e.Button, e.Location);
            _band.MouseClick += (s, e) => HandleClick(e.Button, e.Location);
            _floatingForm.MouseDoubleClick += (s, e) => HandleDoubleClick(e.Button, e.Location);
            _floatingForm.MouseClick += (s, e) => HandleClick(e.Button, e.Location);
            _floatingForm.Move += (s, e) => _tooltipProvider.HandleMove(_floatingForm.Bounds);

            // Console
            var miConsole = _menu.AddItem(Guid.Empty, null, "Console", _console.ToggleConsole);
            _console.OnConsoleToggle += (s, e) => _menu.SetItemCheckedState(miConsole, e.Value);

            // Settings
            _menu.AddItem(Guid.Empty, null, "Settings", () =>
            {
                if (_settingsForm == null)
                {
                    _settingsForm = new SettingsForm(_config, _sp, _console, _modules);
                    _settingsForm.OnApply += (s, e) => { ApplyConfiguration(); _config.Save(); };
                    _settingsForm.FormClosed += (s, e) => { _settingsForm = null; };
                }
                _settingsForm.Show();
                _settingsForm.BringToFront();
            });

            GlobalMouseHook.SetGlobalMouseHook();
            GlobalMouseHook.MouseWheel += (s, e) => HandleMouseWheel(e.Value);
            GlobalMouseHook.MousePoint += (s, e) => HandleMousePoint(e.Value);
            GlobalMouseHook.MousePoint += (s, e) => _tooltipProvider.HandleMousePoint(e.Value);

            ActiveWindowWatcher.StartWatching();
        }

        private void OnClose(object sender, EventArgs e)
        {
            ActiveWindowWatcher.StopWatching();
            GlobalMouseHook.RemoveGlobalMouseHook();

            _config.DisableWatcher();
            _config.Save();
        }

        private void OnResize(object sender, EventArgs e)
        {
            _console.AddDebugLine(String.Format("Module container resized: {0}x{1}", _moduleContainer.Size.Width, _moduleContainer.Size.Height));

            var cfg = _config.GetConfiguration(Guid.Empty, ConfigurationModel.Default);
            if (cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband)
            {
                var tsi = _band.GetTaskbarSizeInfo();
                _band.MinSize = new Size(tsi.Mode == LayoutMode.Horizontal ? _moduleContainer.Size.Width : _moduleContainer.Size.Height, 0);
            }
            else
            {
                _band.MinSize = new Size(BandMinWidth, 0);
            }

            _band.ExecBandInfoChangedCommand();
            _floatingForm.Size = _moduleContainer.Size;

            if (cfg.GeneralSettings.DisplayMode == DisplayMode.FloatingWindow)
            {
                _floatingForm.Visible = _moduleContainer.Size.Width > 0;
            }
        }

        private void ApplyConfiguration()
        {
            var cfg = _config.GetConfiguration(Guid.Empty, ConfigurationModel.Default);
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
                .Select(m => new ModuleSizeInfo(
                    m.Module.Id,
                    m.Configuration.Disabled,
                    _sp.MakeSize(m.Configuration.Width, m.Configuration.Height),
                    _sp.MakePoint(m.Configuration.Offset, 0),
                    m.Configuration.BackgroundImagePath,
                    m.Configuration.StretchBackgroundImage
                 ));

            _moduleContainer.MinWidth = cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband ? BandMinWidth : 0;
            var layoutMode = cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband ? _band.GetTaskbarSizeInfo().Mode : cfg.FloatingWindowSettings.Mode;
            _moduleContainer.UpdateModules(modulesSizeInfo, cfg.GeneralSettings.DrawBorders, layoutMode);

            foreach (var m in _modules)
            {
                m.ApplyConfiguration();
            }

            OnResize(this, EventArgs.Empty);
        }

        private string _lastConfigError = null;
        private void OnConfigurationFileChanged(object sender, EventArgs e)
        {
            _band.Invoke((MethodInvoker)delegate
            {
                try
                {
                    _config.Load();
                    ApplyConfiguration();
                    _lastConfigError = null;
                }
                catch (Exception ex)
                {
                    if (ex.Message != _lastConfigError)
                    {
                        _lastConfigError = ex.Message;
                        _console.AddLine(ex.Message);
                        MessageBox.Show(ex.Message, "Deskband Controls Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            });
        }

        private IModule GetModuleAtPoint(Point location)
        {
            var moduleId = _moduleContainer.LocateModuleAtPoint(location);
            if (moduleId == null)
                return null;
            else
                return _modules.FirstOrDefault(m => m.Id == moduleId.Value);
        }

        private void HandleDoubleClick(MouseButtons button, Point location)
        {
            GetModuleAtPoint(location)?.MouseDoubleClick(button);
        }

        private void HandleClick(MouseButtons button, Point location)
        {
            GetModuleAtPoint(location)?.MouseClick(button);
        }

        private void HandleMouseWheel(WinApiTypes.HookMouseStruct hms)
        {
            if (!_moduleContainer.Created) return;

            var location = _moduleContainer.PointToClient(hms.Point.AsPoint());
            var delta = hms.MouseData;
            var module = GetModuleAtPoint(location);
            module?.MouseWheel(delta);
        }

        private IModule _prevMousePointModule = null;
        private void HandleMousePoint(Point globalPoint)
        {
            if (!_moduleContainer.Created) return;

            var cfg = _config.GetConfiguration(Guid.Empty, ConfigurationModel.Default);
            if (cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband)
            {
                var taskbarInfo = _band.GetTaskbarSizeInfo();
                if (!taskbarInfo.Rect.ToRectangle().Contains(globalPoint)) return; // do not react if point is outside of taskbar
            }

            var location = _moduleContainer.PointToClient(globalPoint);
            var module = GetModuleAtPoint(location);
            if (module != _prevMousePointModule)
            {
                _prevMousePointModule?.MousePointOut();
                _prevMousePointModule = module;
            }
            if (module != null)
            {
                var moduleScreenRectangle = _moduleContainer.GetModuleScreenRectangle(module.Id);
                module.MousePoint(location, globalPoint, moduleScreenRectangle);
            }
        }
    }
}