using kyoseki.Game.Kinematics.Drawables;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;

namespace kyoseki.Game
{
    public class MainScreen : Screen
    {
        [BackgroundDependencyLoader]
        private void load()
        {
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
                }
            };
        }
    }
}
