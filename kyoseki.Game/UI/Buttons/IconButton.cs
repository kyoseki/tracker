using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace kyoseki.Game.UI.Buttons
{
    public class IconButton : KyosekiButton
    {
        public IconUsage Icon
        {
            get => spriteIcon.Icon;
            set => spriteIcon.Icon = value;
        }

        public ColourInfo IconColour
        {
            get => spriteIcon.Colour;
            set => spriteIcon.Colour = value;
        }

        private SpriteIcon spriteIcon;

        public IconButton()
        {
            Child = spriteIcon = new SpriteIcon
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.75f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            };
        }
    }
}
