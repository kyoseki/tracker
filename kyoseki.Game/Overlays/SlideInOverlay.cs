using kyoseki.Game.Input.Bindings;
using kyoseki.Game.UI;
using kyoseki.UI.Components.Buttons;
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
        protected virtual string Title => string.Empty;

        protected override Container<Drawable> Content { get; }

        public const float TITLE_HEIGHT = 44;

        protected SlideInOverlay(bool baseLayout = true)
        {
            RelativeSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Y;

            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;

            if (!baseLayout)
                return;

            InternalChildren = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = TITLE_HEIGHT,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = KyosekiColors.Background.Opacity(0.7f)
                        },
                        new SpriteText
                        {
                            Font = KyosekiFont.Bold.With(size: 28),
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
                    Padding = new MarginPadding { Top = TITLE_HEIGHT },
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = KyosekiColors.Background.Darken(0.5f).Opacity(0.7f)
                    }
                },
                new IconButton
                {
                    Icon = FontAwesome.Solid.Times,
                    IconSize = new Vector2(0.6f),
                    Size = new Vector2(18),
                    Depth = -int.MaxValue,
                    Action = Hide,
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
