using kyoseki.Game.Overlays.SerialMonitor;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace kyoseki.Game.Overlays
{
    public abstract class SlideInOverlay : FocusedOverlayContainer
    {
        public SlideInOverlay()
        {
            RelativeSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Y;

            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Add(new IconButton
            {
                Icon = FontAwesome.Solid.Times,
                IconSize = new Vector2(0.6f),
                Size = new Vector2(SerialTabControl.HEIGHT),
                Action = () =>
                {
                    Hide();
                },
                Origin = Anchor.TopRight,
                Anchor = Anchor.TopRight
            });
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
    }
}
