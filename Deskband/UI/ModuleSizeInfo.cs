using System;
using System.Drawing;

namespace Deskband.UI
{
    public class ModuleSizeInfo
    {
        public Guid Id { get; private set; }
        public bool Disabled { get; private set; }
        public Size Size { get; private set; }
        public Point Offset { get; private set; }
        public string BackgroundImagePath { get; private set; }
        public bool StretchBackgroundImage { get; private set; }

        public ModuleSizeInfo(Guid id, bool disabled, Size size, Point offset, string backgroundImagePath, bool stretchBackgroundImage)
        {
            Id = id;
            Disabled = disabled;
            Size = size;
            Offset = offset;
            BackgroundImagePath = backgroundImagePath;
            StretchBackgroundImage = stretchBackgroundImage;
        }
    }
}
