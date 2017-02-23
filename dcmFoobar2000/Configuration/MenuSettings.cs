using Deskband.Core.Configuration;
using System.ComponentModel;

namespace dcmFoobar2000.Configuration
{
    public class MenuSettings
    {
        [Category("Visibility"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool Enabled { get; set; }

        [Category("Playback"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool Stop { get; set; }

        [Category("Playback"), TypeConverter(typeof(YesNoBooleanConverter))]
        [DisplayName("Play / Pause")]
        public bool PlayPause { get; set; }

        [Category("Playback"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool Previous { get; set; }

        [Category("Playback"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool Next { get; set; }

        [Category("Playback"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool Random { get; set; }

        [Category("Playback"), TypeConverter(typeof(YesNoBooleanConverter))]
        [DisplayName("Toggle Stop After Current")]
        public bool StopAfterCurrent { get; set; }

        [Category("Clipboard"), TypeConverter(typeof(YesNoBooleanConverter))]
        [DisplayName("Copy Artist and Title")]
        public bool CopyArtistAndTitle { get; set; }

        [Category("Clipboard"), TypeConverter(typeof(YesNoBooleanConverter))]
        [DisplayName("Copy Title")]
        public bool CopyTitle { get; set; }

        [Category("Clipboard"), TypeConverter(typeof(YesNoBooleanConverter))]
        [DisplayName("Copy Artist")]
        public bool CopyArtist { get; set; }

        [Category("External"), TypeConverter(typeof(YesNoBooleanConverter))]
        [DisplayName("Open Containing Folder")]
        public bool OpenContainingFolder { get; set; }

        [Category("External"), TypeConverter(typeof(YesNoBooleanConverter))]
        [DisplayName("Search in Internet")]
        public bool SearchInInternet { get; set; }
    }
}
