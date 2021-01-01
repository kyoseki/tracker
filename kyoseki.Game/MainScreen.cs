using kyoseki.Game.Kinematics.Drawables;
using kyoseki.Game.Overlays;
using kyoseki.Game.UI.SerialMonitor;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;

namespace kyoseki.Game
{
    public class MainScreen : Screen
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            SerialMonitorOverlay serial = null;

            InternalChildren = new Drawable[]
            {
                new SensorView
                {
                    RelativeSizeAxes = Axes.Both
                },
                new Box
                {
                    Colour = Colour4.Blue,
                    Size = new osuTK.Vector2(5, 5)
                },
                new BasicButton
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Text = "Open Serial",
                    Action = () =>
                    {
                        serial?.Show();
                    }
                },
                serial = new SerialMonitorOverlay()
            };
        }
    }
}
