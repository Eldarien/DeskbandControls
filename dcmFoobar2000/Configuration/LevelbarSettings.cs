using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dcmFoobar2000.Configuration
{
    public class LevelbarSettings
    {
        public LevelbarSettings()
        {
            PrimarySegmentColor = Color.Lime;
            SecondarySegmentColor = Color.Red;
            InactiveSegmentColor = Color.DimGray;
        }

        [Category("Visibility"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool Visible { get; set; }


        [Category("Position"), DisplayName("Left Channel X")]
        public int LeftChannelX { get; set; }

        [Category("Position"), DisplayName("Left Channel Y")]
        public int LeftChannelY { get; set; }


        [Category("Position"), DisplayName("Right Channel X")]
        public int RightChannelX { get; set; }

        [Category("Position"), DisplayName("Right Channel Y")]
        public int RightChannelY { get; set; }


        [Category("Padding"), DisplayName("Top")]
        public int PaddingTop { get; set; }

        [Category("Padding"), DisplayName("Bottom")]
        public int PaddingBottom { get; set; }


        [Category("Size")]
        public int Width { get; set; }

        [Category("Size")]
        public int Heigth { get; set; }

        [Category("Colors"), DisplayName("Primary Segment Color"), TypeConverter(typeof(ColorHexConverter))]
        public Color PrimarySegmentColor { get; set; }

        [Category("Colors"), DisplayName("Secondary Segment Color"), TypeConverter(typeof(ColorHexConverter))]
        public Color SecondarySegmentColor { get; set; }

        [Category("Colors"), DisplayName("Inactive Segment Color"), TypeConverter(typeof(ColorHexConverter))]
        public Color InactiveSegmentColor { get; set; }

        [Category("Colors"), DisplayName("Background Color"), TypeConverter(typeof(ColorHexConverter))]
        public Color BackgroundColor { get; set; } = Color.Transparent;

        [Category("Look"), DisplayName("Segments Count")]
        public int SegmentsCount { get; set; }

        [Category("Look"), DisplayName("Transition Point (%)")]
        public int TransitionPoint { get; set; }

        [Category("Look"), DisplayName("Striped Segments"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool StripedSegments { get; set; }

        [Category("Look"), DisplayName("Segment/Space Ratio (%)")]
        public int SegmentSpaceRatio { get; set; }

        [Category("Look"), DisplayName("Fade Speed")]
        public int FadeSpeed { get; set; }
    }
}
