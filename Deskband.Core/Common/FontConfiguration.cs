using System;

namespace Deskband.Core.Common
{
    public class FontConfiguration
    {
        public string Name { get; private set; }
        public int Size { get; private set; }
        public FontStyles Styles { get; private set; }

        public FontConfiguration(string name, int size, FontStyles styles)
        {
            Name = name;
            Size = size;
            Styles = styles;
        }
    }

    [Flags]
    public enum FontStyles
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8,
        SemiBold = 16
    }
}
