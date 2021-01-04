using kyoseki.Game.Kinematics.Drawables;
using kyoseki.Game.Overlays.SerialMonitor;
using kyoseki.Game.Overlays.Skeleton;
using kyoseki.Game.Serial;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;

namespace kyoseki.Game
{
    public class MainScreen : Screen
    {
        [BackgroundDependencyLoader]
        private void load(SkeletonLinkManager skeletonLinks, SerialMonitorOverlay serial, SkeletonOverlay skeletons)
        {
            InternalChildren = new Drawable[]
            {
                new TextButton
                {
                    Size = new Vector2(100, 25),
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Text = "Open Serial",
                    Action = () => serial?.Show()
                },
                new TextButton
                {
                    Position = new Vector2(100, 0),
                    Size = new Vector2(125, 25),
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Text = "Open Skeletons",
                    Action = () => skeletons?.Show()
                }
            };
        }

    }
}
