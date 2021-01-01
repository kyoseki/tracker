using System.Linq;
using kyoseki.Game.Serial;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace kyoseki.Game.UI.SerialMonitor
{
    public class SerialChannel : Container
    {
        private ConnectionManager serialConnections;

        private BasicScrollContainer scroll;
        private FillFlowContainer<Message> messageFlow;

        [BackgroundDependencyLoader]
        private void load(ConnectionManager serialConnections)
        {
            this.serialConnections = serialConnections;

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

            serialConnections.MessageReceived += handleMessage;
        }

        private void handleMessage(MessageInfo msg) => Schedule(() =>
        {
            messageFlow.Add(new Message(MessageDirection.Incoming, $"[{msg.Port}] {msg.Content}"));

            if (messageFlow.Count > 150)
            {
                messageFlow.RemoveRange(messageFlow.Children.Take(messageFlow.Count - 150));
            }

            if (scroll.IsScrolledToEnd(messageFlow.DrawHeight / 4))
                scroll.ScrollToEnd();
        });

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            serialConnections.MessageReceived -= handleMessage;
        }
    }
}
