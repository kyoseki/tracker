using kyoseki.Game.Screens.Main;
using kyoseki.Game.Serial;
using osu.Framework.Graphics;
using osu.Framework.Testing;

namespace kyoseki.Game.Tests.Visual.Screens
{
    public class TestSceneSkeletonCard : TestScene
    {
        public TestSceneSkeletonCard()
        {
            Add(new SkeletonCard
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                SkeletonLink = new SkeletonLink
                {
                    Port = "COM5"
                }
            });
        }
    }
}
