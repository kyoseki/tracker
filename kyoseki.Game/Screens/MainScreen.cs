using kyoseki.Game.Overlays.SerialMonitor;
using kyoseki.Game.Overlays.Skeleton;
using kyoseki.Game.Screens.Main;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;

namespace kyoseki.Game.Screens
{
    public class MainScreen : Screen
    {
        [BackgroundDependencyLoader]
        private void load(SerialMonitorOverlay serial, SkeletonOverlay skeletons)
        {
            InternalChildren = new Drawable[]
            {
                new Toolbar
                {
                    RelativeSizeAxes = Axes.X,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    OpenSerial = serial.Show
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = Toolbar.HEIGHT },
                    Children = new Drawable[]
                    {
                        new TextButton
                        {
                            Size = new Vector2(125, 25),
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Text = "Open Skeletons",
                            Action = () => skeletons?.Show()
                        }
                    }
                }
            };
        }
    }
}
