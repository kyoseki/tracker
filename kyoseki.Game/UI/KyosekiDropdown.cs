using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace kyoseki.Game.UI
{
    public class KyosekiDropdown<T> : Dropdown<T>
    {
        protected override DropdownMenu CreateMenu() => new KyosekiDropdownMenu();

        protected override DropdownHeader CreateHeader() => new KyosekiDropdownHeader();

        public class KyosekiDropdownHeader : DropdownHeader
        {
            public const int CORNER_RADIUS = 4;

            public const int VERTICAL_PADDING = 5;
            public const int FONT_SIZE = 18;
            public const int HEIGHT = VERTICAL_PADDING * 2 + FONT_SIZE;

            private readonly SpriteText label;

            protected override string Label
            {
                get => label.Text;
                set => label.Text = value;
            }

            public KyosekiDropdownHeader()
            {
                Foreground.Padding = new MarginPadding
                {
                    Vertical = VERTICAL_PADDING,
                    Horizontal = 10
                };

                BackgroundColour = KyosekiColors.ButtonBackground;
                BackgroundColourHover = KyosekiColors.ButtonSelected;

                CornerRadius = CORNER_RADIUS;

                Depth = -1;

                var font = KyosekiFont.Bold.With(size: FONT_SIZE);

                Children = new Drawable[]
                {
                    label = new SpriteText
                    {
                        AlwaysPresent = true,
                        Font = font,
                        Height = font.Size
                    },
                    new SpriteIcon
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(0.2f),
                        Icon = FontAwesome.Solid.SortDown,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.Centre,
                        X = -10
                    }
                };
            }
        }

        public class KyosekiDropdownMenu : DropdownMenu
        {
            private const int corner_radius = 4;

            private readonly Container gap;

            public KyosekiDropdownMenu()
            {
                BackgroundColour = KyosekiColors.Background.Lighten(0.75f);
                MaskingContainer.CornerRadius = corner_radius;

                AddInternal(gap = new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    CornerRadius = corner_radius,
                    Depth = 1,
                    Masking = true,
                    Child = new Box
                    {
                        Colour = KyosekiColors.ButtonBackground,
                        RelativeSizeAxes = Axes.Both
                    }
                });
            }

            protected override Menu CreateSubMenu() => new KyosekiMenu(Direction.Vertical);

            protected override void AnimateOpen()
            {
                this.FadeIn(KyosekiMenu.FADE_DURATION, Easing.InQuint);

                for (int i = 0; i < Children.Count; i++)
                {
                    ((DrawableKyosekiDropdownMenuItem)Children[i]).Show(i);
                }
            }

            protected override void AnimateClose() => this.FadeOut(KyosekiMenu.FADE_DURATION, Easing.OutQuint);

            protected override void UpdateSize(Vector2 newSize)
            {
                if (Direction == Direction.Vertical)
                {
                    Width = newSize.X;
                    this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
                    gap.Height = newSize.Y + KyosekiDropdownHeader.CORNER_RADIUS + corner_radius;
                }
                else
                {
                    Height = newSize.Y;
                    this.ResizeWidthTo(newSize.X, 300, Easing.OutQuint);
                }
            }

            protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item) => new DrawableKyosekiDropdownMenuItem(item);

            protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new KyosekiScrollContainer(direction);

            private class DrawableKyosekiDropdownMenuItem : DrawableDropdownMenuItem
            {
                public DrawableKyosekiDropdownMenuItem(MenuItem item)
                    : base(item)
                {
                    BackgroundColour = Colour4.Transparent;
                    BackgroundColourHover = KyosekiColors.ButtonSelected;
                    BackgroundColourSelected = KyosekiColors.ButtonSelected.Opacity(0.9f);
                }

                protected override Drawable CreateContent() => new MenuTextContainer();

                public void Show(int idx) => ((MenuTextContainer)Content).Show(idx);
            }
        }
    }
}
