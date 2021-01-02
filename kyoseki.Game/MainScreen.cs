using kyoseki.Game.Kinematics.Drawables;
using kyoseki.Game.Overlays;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osuTK;

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
                new TextButton
                {
                    Size = new Vector2(100, 25),
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Text = "Open Serial",
                    Action = () => serial?.Show()
                },
                serial = new SerialMonitorOverlay()
            };
        }
    }
}
