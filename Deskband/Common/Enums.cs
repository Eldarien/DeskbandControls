using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deskband.Common
{
    public static class Enums
    {
        public enum ButtonKindType
        {
            [Description("Stop")]
            Stop,

            [Description("Play/Pause")]
            PlayPause,

            [Description("Previous")]
            Previous,

            [Description("Next")]
            Next,

            [Description("Random")]
            Random,

            [Description("Stop After Current")]
            StopAfterCurrent
        }

        public enum TrackbarKindType
        {
            [Description("Position")]
            Position,

            [Description("Volume")]
            Volume
        }
    }
}