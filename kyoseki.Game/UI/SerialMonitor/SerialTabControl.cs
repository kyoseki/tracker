using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;

namespace kyoseki.Game.UI.SerialMonitor
{
    public class SerialTabControl : TabControl<string>
    {
        public const float HEIGHT = 30;

        public SerialTabControl()
        {
            RelativeSizeAxes = Axes.X;
            Height = HEIGHT;
        }

        protected override Dropdown<string> CreateDropdown() => new BasicTabControl<string>.BasicTabControlDropdown();

        protected override TabItem<string> CreateTabItem(string value) => new SerialTabItem(value);

        private class SerialTabItem : TabItem<string>
        {
            private Box background;

            private SpriteText text;

            public SerialTabItem(string value)
                : base(value)
            {
                RelativeSizeAxes = Axes.Y;

                Anchor = Anchor.TopLeft;
                Origin = Anchor.TopLeft;

                Width = 100;

                InternalChildren = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.LightGray
                    },
                    text = new SpriteText
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Text = value,
                        Padding = new MarginPadding(5),
                        RelativeSizeAxes = Axes.Y,
                        Truncate = true,
                        Colour = Colour4.Gray
                    }
                };
            }

            protected override void OnActivated()
            {
                text.FadeColour(Colour4.White, 25, Easing.In);
                background.FadeColour(Colour4.DarkGray, 25, Easing.In);
            }

            protected override void OnDeactivated()
            {
                text.FadeColour(Colour4.Gray, 25, Easing.OutQuint);
                background.FadeColour(Colour4.LightGray, 25, Easing.OutQuint);
            }
        }
    }
}
