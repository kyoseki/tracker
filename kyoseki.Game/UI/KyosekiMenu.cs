using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace kyoseki.Game.UI
{
    public class KyosekiMenu : Menu
    {
        public const int FADE_DURATION = 250;

        public KyosekiMenu(Direction direction, bool topLevelMenu = false)
            : base(direction, topLevelMenu)
        {
            BackgroundColour = KyosekiColors.Background.Lighten(0.75f);
            MaskingContainer.CornerRadius = 4;
        }

        protected override Menu CreateSubMenu() => new KyosekiMenu(Direction.Vertical)
        {
            Anchor = Direction == Direction.Horizontal ? Anchor.BottomLeft : Anchor.TopRight
        };

        protected override void AnimateOpen()
        {
            this.FadeIn(FADE_DURATION, Easing.InQuint);

            for (int i = 0; i < Children.Count; i++)
            {
                ((DrawableKyosekiMenuItem)Children[i]).Show(i);
            }
        }

        protected override void AnimateClose() => this.FadeOut(FADE_DURATION, Easing.OutQuad);

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
                BackgroundColourHover = KyosekiColors.ButtonSelected.Opacity(0.5f);
            }

            protected override Drawable CreateContent() => new MenuTextContainer();

            public void Show(int idx) => ((MenuTextContainer)Content).Show(idx);
        }
    }
}
