using Deskband.Core.Common;
using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;

namespace Deskband.Configuration
{
    public class FloatingWindowSettings
    {
        [Category("Position")]
        public int X { get; set; }

        [Category("Position")]
        public int Y { get; set; }

        [DisplayName("Mode")]
        public LayoutMode Mode { get; set; }

        public double Opacity { get; set; }

        public Color Color { get; set; }

        [Category("Background"), DisplayName("Use Background Image"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool UseBackgroundImage { get; set; }

        [Category("Background"), DisplayName("Background Image Path"), Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string BackgroundImagePath { get; set; }

        [Category("Background"), DisplayName("Stretch Background Image"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool StretchBackgroundImage { get; set; }

        [DisplayName("Use Transparency Key"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool UseTransparencyKey { get; set; }

        public static FloatingWindowSettings GetDefault()
        {
            return new FloatingWindowSettings
            {
                Opacity = 1.0,
                Color = Color.Tan
            };
        }
    }
}