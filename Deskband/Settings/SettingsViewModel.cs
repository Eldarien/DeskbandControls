using Deskband.Common;
using Deskband.Settings.Models;
using DeskbandBridge;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Deskband.Settings
{
    internal class SettingsViewModel : ObservableObject<SettingsViewModel>
    {
        private Window _wnd;

        public SettingsData Settings { get; private set; }

        public List<String> FontsList { get; private set; }

        public ObservableCollection<String> Profiles { get; private set; }

        private string _selectedProfile;

        public String SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;
                RaisePropertyChangedEvent(x => x.SelectedProfile);
                RaisePropertyChangedEvent(x => x.SelectedProfileNotNew);
            }
        }

        private const string NewProfile = "< New Profile >";

        public event EventHandler OnClose;

        public event EventHandler OnApply;

        public SettingsViewModel(Window settingsWindow)
        {
            _wnd = settingsWindow;

            Settings = JsonHelpers.CloneObject(SettingsManager.Instance.Settings);

            FontsList = new System.Drawing.Text.InstalledFontCollection().Families
                .Select(x => x.Name)
                .ToList();

            Profiles = new ObservableCollection<String>(SettingsManager.Instance.GetProfiles());
            Profiles.Insert(0, NewProfile);
            SelectedProfile = NewProfile;
        }

        private void ApplySettings()
        {
            SettingsManager.Instance.Settings = Settings;
        }

        public ICommand OK { get { return new DelegateCommand(CommandOK); } }

        private void CommandOK()
        {
            ApplySettings();

            if (OnApply != null)
                OnApply(this, EventArgs.Empty);

            if (OnClose != null)
                OnClose(this, EventArgs.Empty);
        }

        public ICommand Cancel { get { return new DelegateCommand(CommandCancel); } }

        private void CommandCancel()
        {
            if (OnClose != null)
                OnClose(this, EventArgs.Empty);
        }

        public ICommand Apply { get { return new DelegateCommand(CommandApply); } }

        private void CommandApply()
        {
            ApplySettings();

            if (OnApply != null)
                OnApply(this, EventArgs.Empty);
        }

        // TextBlocks

        private int _selectedTextBlockIndex;

        public int SelectedTextBlockIndex
        {
            get { return _selectedTextBlockIndex; }
            set { _selectedTextBlockIndex = value; RaisePropertyChangedEvent(x => x.SelectedTextBlockIndex); }
        }

        public ICommand AddTextBlock { get { return new DelegateCommand(CommandAddTextBlock); } }

        private void CommandAddTextBlock()
        {
            Settings.TextBlocks.Add(new TextBlockModel());
            SelectedTextBlockIndex = Settings.TextBlocks.Count - 1;
        }

        public ICommand RemoveTextBlock { get { return new DelegateCommand(CommandRemoveTextBlock); } }

        private void CommandRemoveTextBlock()
        {
            int index = SelectedTextBlockIndex;
            if (index == -1)
                return;

            Settings.TextBlocks.RemoveAt(index);
            if (index == Settings.TextBlocks.Count)
                SelectedTextBlockIndex = Settings.TextBlocks.Count - 1;
            else
                SelectedTextBlockIndex = index;
        }

        private string ChooseImageFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";
            var result = dlg.ShowDialog();
            return result == true ? dlg.FileName : null;
        }

        // Album Art

        public ICommand AlbumArtStubImageBrowse { get { return new DelegateCommand(CommandAlbumArtStubImageBrowse); } }

        private void CommandAlbumArtStubImageBrowse()
        {
            var fn = ChooseImageFile();
            if (fn != null)
            {
                Settings.AlbumArt.StubImagePath = fn;
            }
        }

        public ICommand AlbumArtStubImageClear { get { return new DelegateCommand(CommandAlbumArtStubImageClear); } }

        private void CommandAlbumArtStubImageClear()
        {
            Settings.AlbumArt.StubImagePath = null;
        }

        // Buttons

        private int _selectedButtonIndex;

        public int SelectedButtonIndex
        {
            get { return _selectedButtonIndex; }
            set
            {
                _selectedButtonIndex = value;
                RaisePropertyChangedEvent(x => x.SelectedButtonIndex);
                RaisePropertyChangedEvent(x => x.ButtonAdditionalIconVisibility);
            }
        }

        public Visibility ButtonAdditionalIconVisibility
        {
            get
            {
                var btn = Settings.Buttons[_selectedButtonIndex];
                return btn.Kind == Enums.ButtonKindType.PlayPause || btn.Kind == Enums.ButtonKindType.StopAfterCurrent
                    ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private string ChooseButtonIconFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.ico|All Files|*.*";
            var result = dlg.ShowDialog();
            return result == true ? dlg.FileName : null;
        }

        public ICommand ButtonIconBrowse { get { return new DelegateCommand(CommandButtonIconBrowse); } }

        private void CommandButtonIconBrowse()
        {
            var fn = ChooseButtonIconFile();
            if (fn != null)
            {
                Settings.Buttons[_selectedButtonIndex].IconPath = fn;
            }
        }

        public ICommand ButtonAdditionalIconBrowse { get { return new DelegateCommand(CommandButtonAdditionalIconBrowse); } }

        private void CommandButtonAdditionalIconBrowse()
        {
            var fn = ChooseButtonIconFile();
            if (fn != null)
            {
                Settings.Buttons[_selectedButtonIndex].AdditionalIconPath = fn;
            }
        }

        public ICommand ButtonIconClear { get { return new DelegateCommand(CommandButtonIconClear); } }

        private void CommandButtonIconClear()
        {
            Settings.Buttons[_selectedButtonIndex].IconPath = null;
        }

        public ICommand ButtonAdditionalIconClear { get { return new DelegateCommand(CommandButtonAdditionalIconClear); } }

        private void CommandButtonAdditionalIconClear()
        {
            Settings.Buttons[_selectedButtonIndex].AdditionalIconPath = null;
        }

        public ICommand FloatingWindowBackgroundImageBrowse { get { return new DelegateCommand(CommandFloatingWindowBackgroundImageBrowse); } }

        private void CommandFloatingWindowBackgroundImageBrowse()
        {
            var fn = ChooseImageFile();
            if (fn != null)
            {
                Settings.FloatingWindow.BackgroundImage = fn;
            }
        }

        public ICommand FloatingWindowBackgroundImageClear { get { return new DelegateCommand(CommandFloatingWindowBackgroundImageClear); } }

        private void CommandFloatingWindowBackgroundImageClear()
        {
            Settings.FloatingWindow.BackgroundImage = null;
        }

        // Profiles

        public ICommand SaveSelectedProfile { get { return new DelegateCommand(CommandSaveSelectedProfile); } }

        private void CommandSaveSelectedProfile()
        {
            string profileName = SelectedProfile;
            if (profileName == NewProfile)
            {
                profileName = PromptDialog.Prompt(_wnd, "Eneter new profile name:", "Create new profile");
            }
            if (String.IsNullOrWhiteSpace(profileName))
                return;

            bool needToAdd = true;
            if (Profiles.Contains(profileName, StringComparer.OrdinalIgnoreCase))
            {
                var msg = String.Format("Profile \"{0}\" already exists. Overwrite?", profileName);
                if (MessageBox.Show(_wnd, msg,
                    FB2KConstants.DeskbandControlsTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning
                    ) == MessageBoxResult.No)
                {
                    return;
                }

                needToAdd = false;
            }

            profileName = SettingsManager.Instance.SaveProfile(profileName, Settings);
            if (needToAdd)
            {
                Profiles.Add(profileName);
                SelectedProfile = profileName;
            }

            CommandApply();
        }

        public ICommand DeleteSelectedProfile { get { return new DelegateCommand(CommandDeleteSelectedProfile); } }

        private void CommandDeleteSelectedProfile()
        {
            var msg = String.Format("Delete \"{0}\" profile?", SelectedProfile);
            if (MessageBox.Show(_wnd, msg,
                FB2KConstants.DeskbandControlsTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning
                ) == MessageBoxResult.Yes)
            {
                SettingsManager.Instance.DeleteProfile(SelectedProfile);
                Profiles.Remove(SelectedProfile);
                SelectedProfile = NewProfile;
            }
        }

        public ICommand LoadSelectedProfile { get { return new DelegateCommand(CommandLoadSelectedProfile); } }

        private void CommandLoadSelectedProfile()
        {
            Settings = SettingsManager.Instance.LoadProfile(SelectedProfile);
            RaisePropertyChangedEvent(x => x.Settings);

            CommandApply();
        }

        public bool SelectedProfileNotNew
        {
            get { return !NewProfile.Equals(SelectedProfile, StringComparison.OrdinalIgnoreCase); }
        }
    }
}