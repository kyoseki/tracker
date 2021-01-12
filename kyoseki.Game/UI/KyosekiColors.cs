using osu.Framework.Graphics;

namespace kyoseki.Game.UI
{
    public static class KyosekiColors
    {
        public static Colour4 ButtonBackground = Colour4.FromHex("1D1E2C");
        public static Colour4 ButtonSelected = Colour4.FromHex("383A56");

        public static Colour4 TextBoxFocused = Colour4.FromHex("383B57");
        public static Colour4 TextBoxUnfocused = TextBoxFocused.Lighten(0.2f);
        public static Colour4 TextBoxReadOnly = Colour4.FromHex("5C5D6C");
        public static Colour4 TextSelected = TextBoxReadOnly.Lighten(0.4f);

        public static Colour4 Background = Colour4.FromHex("181925");
        public static Colour4 Foreground = Colour4.FromHex("F5F5F5");
        public static Colour4 ForegroundSelected = Colour4.White;
    }
}
