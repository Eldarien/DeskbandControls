using Deskband.Settings.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Deskband.Settings
{
    public class SettingsData
    {
        public GeneralModel General { get; set; }

        public FloatingWindowModel FloatingWindow { get; set; }

        public ObservableCollection<TextBlockModel> TextBlocks { get; set; }

        public ObservableCollection<ButtonModel> Buttons { get; set; }

        public ObservableCollection<TrackbarModel> Trackbars { get; set; }

        public AlbumArtModel AlbumArt { get; set; }

        public SettingsData()
        {
            General = new GeneralModel();
            FloatingWindow = new FloatingWindowModel();
            TextBlocks = new ObservableCollection<TextBlockModel>();
            Buttons = new ObservableCollection<ButtonModel>();
            Trackbars = new ObservableCollection<TrackbarModel>();
            AlbumArt = new AlbumArtModel();
        }
    }
}