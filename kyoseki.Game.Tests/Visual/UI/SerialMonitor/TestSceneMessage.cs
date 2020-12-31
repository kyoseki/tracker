using kyoseki.Game.UI.SerialMonitor;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;

namespace kyoseki.Game.Tests.Visual.UI.SerialMonitor
{
    public class TestSceneMessage : TestScene
    {
        public TestSceneMessage()
        {
            Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Message(MessageDirection.Incoming, "Message from device"),
                    new Message(MessageDirection.Outgoing, "Message to device")
                }
            });
        }
    }
}
