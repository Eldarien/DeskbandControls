using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace dcmFoobar2000.Configuration
{
    public class ConfigurationModel : ConfigurationObjectBase
    {
        public ConfigurationModel()
        {
            ModuleId = Foobar2000Module.ModuleId;
            Width = 260;
            Height = 30;
        }

        public override string ToString()
        {
            return Foobar2000Module.ModuleName;
        }

        [Category("General"), DisplayName("Path to folder with foobar2000.exe")]
        public string PathToFoobar2000 { get; set; } = @"%PROGRAMFILES(x86)%\foobar2000";

        [DisplayName("Hide if not playing"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool HideIfNotPlaying { get; set; } = true;

        [DisplayName("Hide if foobar2000 is not running"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool HideIfFoobar2000IsNotRunning { get; set; } = true;

        [Category("Search"), DisplayName("Internet Search Format")]
        public string InternetSearchFormat { get; set; } = "$if(%artist%,%artist%' - ')%title%";

        [Category("Search"), DisplayName("Internet Search URL")]
        public string InternetSearchUrl { get; set; } = "https://www.google.com/search?q=%q%";

        [Browsable(false), SettingsNode("Album Art")]
        public AlbumArtSettings AlbumArt { get; set; } = new AlbumArtSettings { Visible = true, X = 1, Y = 2, Width = 27, Height = 27 };

        [Browsable(false), SettingsNode("Buttons")]
        public ButtonSettingsContainer Buttons { get; set; } = new ButtonSettingsContainer
        {
            BtnStop = new ButtonSettings { Visible = true, X = 30, Y = 1 },
            BtnPlayPause = new ButtonSettings { Visible = true, X = 44, Y = 1 },
            BtnPrev = new ButtonSettings { Visible = true, X = 59, Y = 1 },
            BtnNext = new ButtonSettings { Visible = true, X = 74, Y = 1 },
            BtnRandom = new ButtonSettings { Visible = false, X = 1, Y = 1 },
            BtnStopAC = new ButtonSettings { Visible = false, X = 1, Y = 1 }
        };

        [Browsable(false), SettingsNode("Position Bar")]
        public TrackbarSettings PositionBar { get; set; } = new TrackbarSettings { Visible = true, X = 32, Y = 20, Width = 154, Heigth = 6 };

        [Browsable(false), SettingsNode("Volume Bar")]
        public TrackbarSettings VolumeBar { get; set; } = new TrackbarSettings { Visible = true, X = 190, Y = 20, Width = 60, Heigth = 6 };

        [Browsable(false), SettingsNodeList("Texts")]
        [SettingsCommand("Add Text", nameof(ConfigurationModel.AddText))]
        public List<TextSettings> Texts { get; set; } = new List<TextSettings>();

        [Browsable(false), SettingsNode("Tooltip")]
        public TooltipSettings Tooltip { get; set; } = new TooltipSettings {
            Enabled = true, Width = 400, Height = 400, BackgroundColor = Color.Black, ShowDelay = 1000,
            AlbumArt = new AlbumArtSettings
            {
                Visible = true, X = 0, Y = 0, Width = 400, Height = 400,
                DoNotShowStubImage = false, PreserveAspectRatio = true
            }
        };

        [Browsable(false), SettingsNode("Menu")]
        public MenuSettings Menu { get; set; } = new MenuSettings
        {
            Enabled = true,
            Stop = true,
            PlayPause = true,
            Previous = true,
            Next = true,
            Random = true,
            StopAfterCurrent = true,
            CopyArtistAndTitle = true,
            CopyTitle = true,
            CopyArtist = true,
            OpenContainingFolder = true,
            SearchInInternet = true,
            Playlist = true
        };

        [Browsable(false), SettingsNode("Playlist")]
        public PlaylistSettings Playlist { get; set; } = new PlaylistSettings
        {
            NumberOfItemsBeforeCurrent = 3,
            NumberOfItemsAfterCurrent = 5,
            Format = "%artist% - %title%"
        };

        public static object AddText(ConfigurationModel model)
        {
            return SettingsObject.AddCollectionItemCommandHelper(model.Texts, "Text", name => new TextSettings { Name = name, Format = "%title%" });
        }

        public static object RemoveText(ConfigurationModel model, TextSettings item)
        {
            return SettingsObject.RemoveCollectionItemCommandHelper(model.Texts, item);
        }

        public static ConfigurationModel Default
        {
            get
            {
                var cfg = new ConfigurationModel();
                cfg.Texts.Add(new TextSettings
                {
                    X = 92, Y = 2, Width = 158, Height = 16, Name = "Default Text",
                    StoppedText = "**Stopped**", Format = "%artist% - %title% '('%playback_time%')')"
                });

                cfg.Tooltip.Texts.Add(new TooltipTextSettings
                {
                    Name = "Artist", X = 0, Y = 350, Width = 400, Height = 25, Visible = true,
                    Format = "%artist%", ScrollSeparator = " **** ", ScrollSpeed = 30, ScrollStep = 1,
                    FontName = "Segoe UI", FontSize = 12, FontStyleBold = true, FontColor = Color.White,
                    BackgroundColor = Color.FromArgb(0x90, 0, 0, 0),
                    ShadowColor = Color.Black,
                    HorizontalAlign = HorizontalAlign.Center
                });
                cfg.Tooltip.Texts.Add(new TooltipTextSettings
                {
                    Name = "Title", X = 0, Y = 375, Width = 400, Height = 25, Visible = true,
                    Format = "%title%", ScrollSeparator = " **** ", ScrollSpeed = 30, ScrollStep = 1,
                    FontName = "Segoe UI", FontSize = 12, FontStyleBold = true, FontColor = Color.White,
                    BackgroundColor = Color.FromArgb(0x90, 0, 0, 0),
                    ShadowColor = Color.Black,
                    HorizontalAlign = HorizontalAlign.Center
                });

                return cfg;
            }
        }
    }
}