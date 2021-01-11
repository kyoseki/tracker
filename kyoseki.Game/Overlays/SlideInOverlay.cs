using kyoseki.Game.Input.Bindings;
using kyoseki.Game.UI;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osuTK;

namespace kyoseki.Game.Overlays
{
    public abstract class SlideInOverlay : FocusedOverlayContainer, IKeyBindingHandler<GlobalAction>
    {
        protected virtual string Title { get; }

        protected override Container<Drawable> Content { get; }

        private const float title_height = 44;

        public SlideInOverlay()
        {
            RelativeSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Y;

            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;

            InternalChildren = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = title_height,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = KyosekiColors.BACKGROUND.Opacity(0.7f)
                        },
                        new SpriteText
                        {
                            Font = new FontUsage("Manrope", 28, "Bold"),
                            Padding = new MarginPadding { Left = 18 },
                            RelativeSizeAxes = Axes.X,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = Title
                        }
                    }
                },
                Content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = title_height },
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = KyosekiColors.BACKGROUND.Darken(0.5f).Opacity(0.7f)
                    }
                },
                new IconButton
                {
                    Icon = FontAwesome.Solid.Times,
                    IconSize = new Vector2(0.6f),
                    Size = new Vector2(18),
                    Action = () =>
                    {
                        Hide();
                    },
                    Origin = Anchor.TopRight,
                    Anchor = Anchor.TopRight
                }
            };
        }

        protected override void PopIn()
        {
            this.MoveToY(0, 250, Easing.OutQuint);

            base.PopIn();
        }

        protected override void PopOut()
        {
            this.MoveToY(2, 250, Easing.In);

            base.PopOut();
        }

        public bool OnPressed(GlobalAction action)
        {
            switch (action)
            {
                case GlobalAction.Back:
                    Hide();
                    return true;
            }

            return false;
        }

        public void OnReleased(GlobalAction action)
        {
        }
    }
}
