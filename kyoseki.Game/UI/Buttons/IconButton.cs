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

        public Vector2 IconSize
        {
            get => spriteIcon.Size;
            set => spriteIcon.Size = value;
        }

        protected virtual SpriteIcon CreateIcon() => new SpriteIcon
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.75f),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Colour = KyosekiColors.Foreground
        };

        private readonly SpriteIcon spriteIcon;

        public IconButton()
        {
            Child = spriteIcon = CreateIcon();
        }
    }
}
