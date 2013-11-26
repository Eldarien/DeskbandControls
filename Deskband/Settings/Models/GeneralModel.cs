using Deskband.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Deskband.Settings.Models
{
    public class GeneralModel : ObservableObject<GeneralModel>
    {
        private int _bandSize;

        public int BandSize
        {
            get { return _bandSize; }
            set { _bandSize = value; RaisePropertyChangedEvent(x => x.BandSize); }
        }

        private int _textScrollSpeed;

        public int TextScrollSpeed
        {
            get { return _textScrollSpeed; }
            set { _textScrollSpeed = value; RaisePropertyChangedEvent(x => x.TextScrollSpeed); }
        }

        private bool _drawControlsOutline;

        public bool DrawControlsOutline
        {
            get { return _drawControlsOutline; }
            set { _drawControlsOutline = value; RaisePropertyChangedEvent(x => x.DrawControlsOutline); }
        }

        private bool _hideIfNotPlaying;

        public bool HideIfNotPlaying
        {
            get { return _hideIfNotPlaying; }
            set { _hideIfNotPlaying = value; RaisePropertyChangedEvent(x => x.HideIfNotPlaying); }
        }

        private bool _hideIfFoobar2000IsNotRunning;

        public bool HideIfFoobar2000IsNotRunning
        {
            get { return _hideIfFoobar2000IsNotRunning; }
            set { _hideIfFoobar2000IsNotRunning = value; RaisePropertyChangedEvent(x => x.HideIfFoobar2000IsNotRunning); }
        }

        private bool _floatingMode;

        public bool FloatingMode
        {
            get { return _floatingMode; }
            set { _floatingMode = value; RaisePropertyChangedEvent(x => x.FloatingMode); }
        }

        private string _internetSearchFormat;

        public string InternetSearchFormat
        {
            get { return _internetSearchFormat; }
            set { _internetSearchFormat = value; RaisePropertyChangedEvent(x => x.InternetSearchFormat); }
        }

        private string _internetSearchUrl;

        public string InternetSearchUrl
        {
            get { return _internetSearchUrl; }
            set { _internetSearchUrl = value; RaisePropertyChangedEvent(x => x.InternetSearchUrl); }
        }
    }
}