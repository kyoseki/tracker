using kyoseki.Game.UI;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;

namespace kyoseki.Game.Overlays.SerialMonitor
{
    public class SerialTabControl : TabControl<string>
    {
        public const float HEIGHT = 30;

        private const int fade_duration = 100;

        public SerialTabControl()
        {
            RelativeSizeAxes = Axes.X;
            Height = HEIGHT;
        }

        protected override Dropdown<string> CreateDropdown() => new BasicTabControl<string>.BasicTabControlDropdown();

        protected override TabItem<string> CreateTabItem(string value) => new SerialTabItem(value);

        private class SerialTabItem : TabItem<string>
        {
            private readonly Box background;

            private readonly SpriteText text;

            public SerialTabItem(string value)
                : base(value)
            {
                RelativeSizeAxes = Axes.Y;

                Anchor = Anchor.TopLeft;
                Origin = Anchor.TopLeft;

                Width = 80;

                InternalChildren = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = KyosekiColors.ButtonBackground
                    },
                    text = new SpriteText
                    {
                        Font = KyosekiFont.GetFont(size: 20),
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Text = value,
                        Padding = new MarginPadding(5),
                        Truncate = true,
                        Colour = KyosekiColors.Foreground
                    }
                };
            }

            protected override void OnActivated()
            {
                text.FadeColour(KyosekiColors.ForegroundSelected, fade_duration, Easing.In);
                background.FadeColour(KyosekiColors.ButtonSelected, fade_duration, Easing.In);
            }

            protected override void OnDeactivated()
            {
                text.FadeColour(KyosekiColors.Foreground, fade_duration, Easing.OutQuint);
                background.FadeColour(KyosekiColors.ButtonBackground, fade_duration, Easing.OutQuint);
            }
        }
    }
}
