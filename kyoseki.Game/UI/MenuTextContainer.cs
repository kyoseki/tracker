using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace kyoseki.Game.UI
{
    internal class MenuTextContainer : Container, IHasText
    {
        private const int margin_horizontal = 7;

        private const int margin_vertical = 3;

        public string Text
        {
            get => text.Text;
            set => text.Text = value;
        }

        private readonly SpriteText text;

        public MenuTextContainer()
        {
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.CentreLeft;

            Masking = true;

            AutoSizeAxes = Axes.Y;
            Child = text = new SpriteText
            {
                RelativePositionAxes = Axes.X,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Horizontal = margin_horizontal, Vertical = margin_vertical },
                Font = KyosekiFont.GetFont(size: 15),
                X = -1
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Width = text.DrawWidth + margin_horizontal * 2;
        }

        public void Show(int idx)
        {
            var addDelay = Math.Min(160, 20 * idx);

            text.X = -1;
            text.Delay(KyosekiMenu.FADE_DURATION / 1.25 + addDelay).MoveToX(0, 200, Easing.OutExpo);
        }
    }
}
