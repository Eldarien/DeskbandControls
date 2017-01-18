using Deskband.Core.Common;
using Deskband.Core.Configuration;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace dcmFoobar2000.Configuration
{
    public class AlbumArtSettings
    {
        [Category("Visibility"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool Visible { get; set; }
        
        [Category("Position")]
        public int X { get; set; }

        [Category("Position")]
        public int Y { get; set; }

        [Category("Size")]
        public int Width { get; set; }

        [Category("Size")]
        public int Height { get; set; }

        [Category("Image"), DisplayName("Stub Image Path"), Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string StubImagePath { get { return _stubImagePath; } set { _stubImagePath = PathHelpers.TryPlaceEnvVars(value); } }
        private string _stubImagePath;

        [Category("Image"), DisplayName("Do Not Use Stub Image"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool DoNotShowStubImage { get; set; }

        [Category("Image"), DisplayName("Preserve Aspect Ratio"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool PreserveAspectRatio { get; set; }
    }
}