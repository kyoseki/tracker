﻿using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;

namespace kyoseki.Game.UI.Buttons
{
    public class TextButton : KyosekiButton
    {
        public string Text
        {
            get => spriteText.Text;
            set => spriteText.Text = value;
        }

        public ColourInfo TextColour
        {
            get => spriteText.Colour;
            set => spriteText.Colour = value;
        }

        private SpriteText spriteText;

        public TextButton()
        {
            Child = spriteText = new SpriteText
            {
                Truncate = true,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = KyosekiColors.FOREGROUND
            };
        }
    }
}