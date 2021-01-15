using kyoseki.Game.Overlays.Skeleton;
using NUnit.Framework;

namespace kyoseki.Game.Tests.Visual.Overlays
{
    public class TestSceneSkeletonOverlay : KyosekiTestScene
    {
        private readonly SkeletonOverlay overlay;

        public TestSceneSkeletonOverlay()
        {
            Add(overlay = new SkeletonOverlay());
        }

        [Test]
        public void TestShow()
        {
            AddStep("Show", overlay.Show);
        }

        [Test]
        public void TestHide()
        {
            AddStep("Hide", overlay.Hide);
        }
    }
}
