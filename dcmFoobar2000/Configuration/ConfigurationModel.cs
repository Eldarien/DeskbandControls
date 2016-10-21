using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Configuration
{
    public class ConfigurationModel : ConfigurationObjectBase
    {
        public override string ToString()
        {
            return Foobar2000Module.ModuleName;
        }

        [DisplayName("Hide if not playing"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool HideIfNotPlaying { get; set; }

        [DisplayName("Hide if foobar2000 is not running"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool HideIfFoobar2000IsNotRunning { get; set; }

        [Category("Search"), DisplayName("Internet Search Format")]
        public string InternetSearchFormat { get; set; }

        [Category("Search"), DisplayName("Internet Search URL")]
        public string InternetSearchUrl { get; set; }

        [DisplayName("Text Scroll Speed")]
        public int TextScrollSpeed { get; set; }

        [Browsable(false), SettingsNode("Album Art")]
        public AlbumArtSettings AlbumArt { get; set; }

        [Browsable(false), SettingsNode("Buttons")]
        public ButtonSettingsContainer Buttons { get; set; }

        [Browsable(false), SettingsNode("Position Bar")]
        public TrackbarSettings PositionBar { get; set; }

        [Browsable(false), SettingsNode("Volume Bar")]
        public TrackbarSettings VolumeBar { get; set; }

        [Browsable(false), SettingsNodeList("Texts")]
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
            Width = 260,
            Height = 30,

            InternetSearchFormat = "$if(%artist%,%artist%' - ')%title%",
            InternetSearchUrl = "https://www.google.com/search?q=%q%",
            TextScrollSpeed = 300,

            AlbumArt = new AlbumArtSettings { Visible = true, X = 1, Y = 2, Width = 27, Height = 27 },

            Buttons = new ButtonSettingsContainer
            {
                BtnStop = new ButtonSettings { Visible = true, X = 30, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },
                BtnPlayPause = new ButtonSettings { Visible = true, X = 44, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },
                BtnPrev = new ButtonSettings { Visible = true, X = 59, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },
                BtnNext = new ButtonSettings { Visible = true, X = 74, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },
                BtnRandom = new ButtonSettings { Visible = false, X = 1, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White },
                BtnStopAC = new ButtonSettings { Visible = false, X = 1, Y = 1, Width = 16, Height = 16, ColorizeColor = Color.White }
            },

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