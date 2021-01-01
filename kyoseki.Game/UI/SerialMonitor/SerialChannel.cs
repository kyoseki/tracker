using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace kyoseki.Game.UI.SerialMonitor
{
    public class SerialChannel : Container
    {
        private readonly BasicScrollContainer scroll;

        private readonly FillFlowContainer<Message> messageFlow;

        public readonly string Port;

        public SerialChannel(string port)
        {
            Port = port;

            Child = scroll = new BasicScrollContainer
            {
                ScrollbarVisible = true,
                RelativeSizeAxes = Axes.Both,
                Child = messageFlow = new FillFlowContainer<Message>
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical
                }
            };
        }

        public void AddMessage(MessageDirection direction, string msg) => Schedule(() =>
        {
            messageFlow.Add(new Message(direction, msg));

            if (messageFlow.Count > 150)
            {
                messageFlow.RemoveRange(messageFlow.Children.Take(messageFlow.Count - 150));
            }

            if (scroll.IsScrolledToEnd(messageFlow.DrawHeight / 6))
                scroll.ScrollToEnd();
        });
    }
}
