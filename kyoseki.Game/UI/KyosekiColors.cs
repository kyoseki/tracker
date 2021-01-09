using osu.Framework.Graphics;

namespace kyoseki.Game.UI
{
    public static class KyosekiColors
    {
        public static Colour4 BUTTON_BACKGROUND = Colour4.FromHex("1D1E2C");
        public static Colour4 BUTTON_SELECTED = Colour4.FromHex("383A56");

        public static Colour4 TEXTBOX_FOCUSED = Colour4.FromHex("383B57");
        public static Colour4 TEXTBOX_UNFOCUSED = TEXTBOX_FOCUSED.Lighten(0.2f);
        public static Colour4 TEXTBOX_READONLY = Colour4.FromHex("5C5D6C");
        public static Colour4 TEXT_SELECTED = TEXTBOX_READONLY.Lighten(0.4f);

        public static Colour4 BACKGROUND = Colour4.FromHex("181925");
        public static Colour4 FOREGROUND = Colour4.FromHex("F5F5F5");
        public static Colour4 FOREGROUND_SELECTED = Colour4.White;
    }
}
