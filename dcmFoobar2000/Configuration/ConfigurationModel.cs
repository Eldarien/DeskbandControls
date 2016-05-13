using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;

namespace dcmFoobar2000.Configuration
{
    public class ConfigurationModel : ConfigurationObjectBase
    {
        public override string ToString()
        {
            return Foobar2000Module.ModuleName;
        }

        public bool HideIfNotPlaying { get; set; }
        public bool HideIfFoobar2000IsNotRunning { get; set; }
        public string InternetSearchFormat { get; set; }
        public string InternetSearchUrl { get; set; }
        public int TextScrollSpeed { get; set; }

        public AlbumArtSettings AlbumArt { get; set; }

        public ButtonSettings BtnStop { get; set; }
        public ButtonSettings BtnPlayPause { get; set; }
        public ButtonSettings BtnPrev { get; set; }
        public ButtonSettings BtnNext { get; set; }
        public ButtonSettings BtnRandom { get; set; }
        public ButtonSettings BtnStopAC { get; set; }

        public TrackbarSettings PositionBar { get; set; }
        public TrackbarSettings VolumeBar { get; set; }

        [Description("Text labels with dynamic text (using foobar2000 formatting syntax)")]
        [SettingsCommand("Add Text", nameof(ConfigurationModel.AddText))]
        public SettingsList<TextSettings> Texts { get; set; }

        public static void AddText(ConfigurationModel model)
        {
            //model.Texts.Add(new TextSettings());
        }

        public static void RemoveText(ConfigurationModel model, TextSettings item)
        {
        }

        public static readonly ConfigurationModel Default = new ConfigurationModel
        {
            ModuleId = Foobar2000Module.ModuleId,

            InternetSearchFormat = "$if(%artist%,%artist%' - ')%title%",
            InternetSearchUrl = "https://www.google.com/search?q=%q%",
            TextScrollSpeed = 300,

            AlbumArt = new AlbumArtSettings { Visible = true, X = 1, Y = 2, Width = 27, Height = 27 },

            BtnStop = new ButtonSettings { Visible = true, X = 30, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },
            BtnPlayPause = new ButtonSettings { Visible = true, X = 44, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },
            BtnPrev = new ButtonSettings { Visible = true, X = 59, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },
            BtnNext = new ButtonSettings { Visible = true, X = 74, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },
            BtnRandom = new ButtonSettings { Visible = false, X = 1, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },
            BtnStopAC = new ButtonSettings { Visible = false, X = 1, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },

            PositionBar = new TrackbarSettings { Visible = true, X = 32, Y = 20, Width = 154, Heigth = 6, Color = Color.White },
            VolumeBar = new TrackbarSettings { Visible = true, X = 190, Y = 20, Width = 60, Heigth = 6, Color = Color.White },

            Texts = new SettingsList<TextSettings>
            {
                new TextSettings { Name = "Default Text", Visible = true, X = 92, Y = 2, Width = 158, Height = 16, EnableScroll = true, StoppedText = "**Stopped**",
                    Format = "%artist% - %title% '('%playback_time%')')", FontName = "Segoe UI", FontSize = 8, FontColor = Color.White }
            }
        };
    }
}