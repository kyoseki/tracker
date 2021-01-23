using kyoseki.Game.Overlays.SerialMonitor;
using kyoseki.Game.Serial;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Testing;

namespace kyoseki.Game.Tests.Visual.Overlays
{
    public class TestSceneSerialChannel : TestScene
    {
        private readonly SerialChannel channel;

        public TestSceneSerialChannel()
        {
            Add(channel = new SerialChannel
            {
                PortName = "TEST",
                RelativeSizeAxes = Axes.Both
            });
        }

        [Test]
        public void TestAddMessages()
        {
            int messages = 0;

            AddRepeatStep("Add messages", () =>
            {
                for (int i = 0; i < 100; i++)
                {
                    channel.AddMessage(new MessageInfo
                    {
                        Direction = messages % 2 == 0 ? MessageDirection.Incoming : MessageDirection.Outgoing,
                        Content = $"brrrrr\n{messages}\nwwww"
                    });

                    messages++;
                }
            }, 10);
        }
    }
}
