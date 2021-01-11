using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace kyoseki.Game.UI
{
    public class KyosekiMenu : Menu
    {
        private const int fade_duration = 250;

        public KyosekiMenu(Direction direction, bool topLevelMenu = false)
            : base(direction, topLevelMenu)
        {
            BackgroundColour = KyosekiColors.BACKGROUND.Lighten(0.75f);
            MaskingContainer.CornerRadius = 4;
        }

        protected override Menu CreateSubMenu() => new KyosekiMenu(Direction.Vertical)
        {
            Anchor = Direction == Direction.Horizontal ? Anchor.BottomLeft : Anchor.TopRight
        };

        protected override void AnimateOpen()
        {
            this.FadeIn(fade_duration, Easing.InQuint);

            for (int i = 0; i < Children.Count; i++)
            {
                ((DrawableKyosekiMenuItem)Children[i]).Show(i);
            }
        }

        protected override void AnimateClose() => this.FadeOut(fade_duration, Easing.OutQuad);

        protected override void UpdateSize(Vector2 newSize)
        {
            if (Direction == Direction.Vertical)
            {
                Width = newSize.X;
                this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
            }
            else
            {
                Height = newSize.Y;
                this.ResizeWidthTo(newSize.X, 300, Easing.OutQuint);
            }
        }

        protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableKyosekiMenuItem(item);

        protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new KyosekiScrollContainer(direction);

        public class DrawableKyosekiMenuItem : DrawableMenuItem
        {
            public DrawableKyosekiMenuItem(MenuItem item)
                : base(item)
            {
                BackgroundColour = Colour4.Transparent;
                BackgroundColourHover = KyosekiColors.BUTTON_SELECTED.Opacity(0.5f);
            }

            protected override Drawable CreateContent() => new TextContainer();

            public void Show(int idx) => ((TextContainer)Content).Show(idx);

            private class TextContainer : Container, IHasText
            {
                private const int margin_horizontal = 7;

                private const int margin_vertical = 3;

                public string Text
                {
                    get => text.Text;
                    set => text.Text = value;
                }

                private readonly SpriteText text;

                public TextContainer()
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
                    text.Delay(fade_duration / 1.25 + addDelay).MoveToX(0, 200, Easing.OutExpo);
                }
            }
        }
    }
}