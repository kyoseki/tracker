using System;
using System.Collections.Generic;
using System.Linq;
using kyoseki.Game.Serial;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace kyoseki.Game.Overlays.SerialMonitor
{
    public class SerialMonitorOverlay : SlideInOverlay
    {
        protected override string Title => "Serial Monitor";

        private ConnectionManager serialConnections;

        private SerialTabControl tabControl;

        private readonly List<SerialChannel> loadedChannels = new List<SerialChannel>();

        private Container tabContent;

        [BackgroundDependencyLoader]
        private void load(ConnectionManager serialConnections)
        {
            this.serialConnections = serialConnections;

            AddRange(new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding
                    {
                        Top = SerialTabControl.HEIGHT
                    },
                    Children = new Drawable[]
                    {
                        tabContent = new Container { RelativeSizeAxes = Axes.Both }
                    }
                },
                tabControl = new SerialTabControl()
            });

            handlePortsUpdated(serialConnections.PortNames.ToArray(), Array.Empty<string>());

            serialConnections.PortsUpdated += handlePortsUpdated;
            serialConnections.MessageReceived += handleMessage;
            serialConnections.MessageSent += handleMessage;
            tabControl.Current.ValueChanged += handleTabChanged;
        }

        private void handleTabChanged(ValueChangedEvent<string> e)
        {
            var tab = loadedChannels.Find(c => c.PortName == e.NewValue);
            tabContent.FadeOut(500, Easing.OutQuint);
            LoadComponentAsync(tab, t =>
            {
                tabContent.Clear(false);
                tabContent.Add(t);
                tabContent.FadeIn(500, Easing.In);
            });
        }

        private void handlePortsUpdated(string[] added, string[] removed) => Schedule(() =>
        {
            foreach (var port in added)
            {
                if (loadedChannels.All(c => c.PortName != port))
                {
                    tabControl.AddItem(port);
                    loadedChannels.Add(new SerialChannel(serialConnections.Get(port))
                    {
                        RelativeSizeAxes = Axes.Both,
                        PortState = { BindTarget = serialConnections.Get(port).State },
                        SendMessage = serialConnections.SendMessage
                    });
                }
            }
        });

        private void handleMessage(MessageInfo msg) => Schedule(() =>
        {
            var channel = loadedChannels.Find(c => c.PortName == msg.Port);
            channel.AddMessage(msg);
        });

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            serialConnections.PortsUpdated -= handlePortsUpdated;
            serialConnections.MessageReceived -= handleMessage;
            serialConnections.MessageSent -= handleMessage;
        }
    }
}
