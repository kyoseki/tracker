using kyoseki.Game.Serial;
using kyoseki.Game.Overlays.SerialMonitor;
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
                    new Message
                    {
                        Item = new MessageInfo
                        {
                            Direction = MessageDirection.Incoming,
                            Content = "Message from device"
                        }
                    },
                    new Message
                    {
                        Item = new MessageInfo
                        {
                            Direction = MessageDirection.Outgoing,
                            Content = "Message to device"
                        }
                    }
                }
            });
        }
    }
}
