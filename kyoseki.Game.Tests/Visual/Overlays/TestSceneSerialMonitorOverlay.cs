using kyoseki.Game.Overlays.SerialMonitor;
using NUnit.Framework;

namespace kyoseki.Game.Tests.Visual.Overlays
{
    public class TestSceneSerialMonitorOverlay : KyosekiTestScene
    {
        private readonly SerialMonitorOverlay overlay;

        public TestSceneSerialMonitorOverlay()
        {
            Add(overlay = new SerialMonitorOverlay());
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