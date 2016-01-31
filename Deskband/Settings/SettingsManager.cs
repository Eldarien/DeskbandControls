using Deskband.Common;
using Deskband.Common.Extensions;
using Deskband.Settings.Models;
using DeskbandBridge;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Deskband.Settings
{
    public class SettingsManager
    {
        public SettingsData Settings { get; set; }

        public StateData State { get; set; }

        private string _appProfileDir;
        private string _configFilePath;
        private string _stateFilePath;
        private string _profilesPath;

        public SettingsManager()
        {
            var _assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _appProfileDir = Path.Combine(System.Environment.GetEnvironmentVariable("AppData"), "DeskbandControls");
            if (!Directory.Exists(_appProfileDir))
                Directory.CreateDirectory(_appProfileDir);

            _profilesPath = Path.Combine(_appProfileDir, "Profiles");
            if (!Directory.Exists(_profilesPath))
            {
                Directory.CreateDirectory(_profilesPath);
                var defaultProfilesDir = Path.Combine(_assemblyDir, "DefaultProfiles");
                var defaultProfiles = Directory.GetFiles(defaultProfilesDir, "*.js");
                foreach (var dp in defaultProfiles)
                {
                    File.Copy(dp, Path.Combine(_profilesPath, Path.GetFileName(dp)));
                }
            }

            _configFilePath = Path.Combine(_appProfileDir, "config.js");
            _stateFilePath = Path.Combine(_appProfileDir, "state.js");

            Settings = GetDefaultSettings();
            State = GetDefaultState();
        }

        public List<String> GetProfiles()
        {
            try
            {
                return Directory.GetFiles(_profilesPath, "*.js")
                    .Select(x => Path.GetFileNameWithoutExtension(x))
                    .OrderBy(x => x)
                    .ToList();
            }
            catch
            {
                return new List<String>();
            }
        }

        private string GetProfileFilePath(string profileName)
        {
            return Path.Combine(_profilesPath, profileName.SanitizeFileName()) + ".js";
        }

        public string SaveProfile(string profileName, SettingsData settingsData)
        {
            var profilePath = GetProfileFilePath(profileName);
            SaveSettingsData(settingsData, profilePath);
            return Path.GetFileNameWithoutExtension(profilePath);
        }

        public SettingsData LoadProfile(string profileName)
        {
            var profilePath = GetProfileFilePath(profileName);
            return LoadSettingsData(profilePath, GetDefaultSettings());
        }

        public void DeleteProfile(string profileName)
        {
            var profilePath = GetProfileFilePath(profileName);
            if (File.Exists(profilePath))
            {
                File.Delete(profilePath);
            }
        }

        private T LoadSettingsData<T>(string settingsFilePath, T defaults)
        {
            T settingsData;

            if (File.Exists(settingsFilePath))
            {
                try
                {
                    string json = File.ReadAllText(settingsFilePath);
                    var savedData = (JObject)JsonConvert.DeserializeObject(json);
                    //var newData = JObject.FromObject(defaults);
                    //JsonHelpers.Merge(newData, savedData);
                    settingsData = savedData.ToObject<T>();
                }
                catch (Exception ex)
                {
                    string msg = String.Format("Unable to parse configuration file\n\"{0}\"\n{1}", settingsFilePath, ex.Message);
                    MessageBox.Show(msg, FB2KConstants.DeskbandControlsTitle, MessageBoxButton.OK, MessageBoxImage.Error);

                    settingsData = defaults;
                }
            }
            else
            {
                settingsData = defaults;
            }

            return settingsData;
        }

        private void SaveSettingsData<T>(T settingsData, string settingsFilePath)
        {
            string json = JsonConvert.SerializeObject(settingsData, Formatting.Indented);
            File.WriteAllText(settingsFilePath, json);
        }

        public void LoadSettings()
        {
            Settings = LoadSettingsData(_configFilePath, GetDefaultSettings());
            //State = LoadSettingsData(_stateFilePath, GetDefaultState());
        }

        public void SaveSettings()
        {
            SaveSettingsData(Settings, _configFilePath);
            //SaveSettingsData(State, _stateFilePath);
        }

        private SettingsData GetDefaultSettings()
        {
            string fontName = Environment.OSVersion.Version.Major < 6 ? "Tahoma" : "Segoe UI";

            var settings = new SettingsData();

            settings.General.BandSize = 250;
            settings.General.TextScrollSpeed = 300;
            settings.General.DrawControlsOutline = false;
            settings.General.FloatingMode = false;
            settings.General.InternetSearchFormat = "$if(%artist%,%artist%' - ')%title%";
            settings.General.InternetSearchUrl = "https://www.google.com/search?q=%q%";

            settings.FloatingWindow.Opacity = 1.0;
            settings.FloatingWindow.Width = 250;
            settings.FloatingWindow.Height = 50;
            settings.FloatingWindow.Color = Colors.DarkSlateGray;
            settings.FloatingWindow.BackgroundImage = null;
            settings.FloatingWindow.UseBackgroundImage = false;

            settings.TextBlocks.Add(new TextBlockModel("Primary Text", "%artist% - %title% '('%playback_time%')')", fontName, 9, Colors.White, 100, 5, 150, 16) { StoppedText = "**Stopped**" });

            settings.Buttons.Add(new ButtonModel(Enums.ButtonKindType.Stop, 38, 4) { IconPath = IconPath("Stop.ico") });
            settings.Buttons.Add(new ButtonModel(Enums.ButtonKindType.PlayPause, 53, 4) { IconPath = IconPath("Play.ico"), AdditionalIconPath = IconPath("Pause.ico") });
            settings.Buttons.Add(new ButtonModel(Enums.ButtonKindType.Previous, 68, 4) { IconPath = IconPath("Prev.ico") });
            settings.Buttons.Add(new ButtonModel(Enums.ButtonKindType.Next, 83, 4) { IconPath = IconPath("Next.ico") });
            settings.Buttons.Add(new ButtonModel(Enums.ButtonKindType.StopAfterCurrent, 0, 0) { Visible = false, IconPath = IconPath("StopAfterCurrentOff.ico"), AdditionalIconPath = IconPath("StopAfterCurrentOn.ico") });
            settings.Buttons.Add(new ButtonModel(Enums.ButtonKindType.Random, 0, 0) { Visible = false, IconPath = IconPath("Random.ico") });

            settings.Trackbars.Add(new TrackbarModel(Enums.TrackbarKindType.Position, Colors.White, 40, 25, 148, 6));
            settings.Trackbars.Add(new TrackbarModel(Enums.TrackbarKindType.Volume, Colors.White, 190, 25, 60, 6));

            settings.AlbumArt.X = 0;
            settings.AlbumArt.Y = 2;
            settings.AlbumArt.Width = 37;
            settings.AlbumArt.Height = 37;
            settings.AlbumArt.Visible = true;

            return settings;
        }

        private StateData GetDefaultState()
        {
            var state = new StateData();
            state.LastUpdateCheck = DateTime.MinValue;
            return state;
        }

        private string IconPath(string fileName)
        {
            return Path.Combine(_appProfileDir, "Icons", fileName);
        }
    }
}