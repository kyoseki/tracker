using kyoseki.Game.Screens.Main;
using osu.Framework.Graphics;
using osu.Framework.Testing;

namespace kyoseki.Game.Tests.Visual.Screens
{
    public class TestSceneToolbar : TestScene
    {
        public TestSceneToolbar()
        {
            Add(new Toolbar
            {
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            });
        }
    }
}
