using System;
using System.Collections.Generic;
using System.Linq;
using kyoseki.Game.Serial;
using kyoseki.Game.UI;
using kyoseki.Game.UI.Buttons;
using kyoseki.Game.UI.SerialMonitor;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace kyoseki.Game.Overlays
{
    public class SerialMonitorOverlay : FocusedOverlayContainer
    {
        private ConnectionManager serialConnections;

        private SerialTabControl tabControl;

        private List<SerialChannel> loadedChannels = new List<SerialChannel>();

        private Container tabContent;

        public SerialMonitorOverlay()
        {
            RelativeSizeAxes = Axes.Both;
            RelativePositionAxes = Axes.Y;

            Anchor = Anchor.BottomCentre;
            Origin = Anchor.BottomCentre;
        }

        [BackgroundDependencyLoader]
        private void load(ConnectionManager serialConnections)
        {
            this.serialConnections = serialConnections;

            Children = new Drawable[]
            {
                new Box
                {
                    Colour = KyosekiColors.BACKGROUND.Darken(0.2f).Opacity(0.5f),
                    RelativeSizeAxes = Axes.Both
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding
                    {
                        Top = SerialTabControl.HEIGHT
                    },
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = KyosekiColors.BACKGROUND.Opacity(0.5f)
                        },
                        tabContent = new Container { RelativeSizeAxes = Axes.Both }
                    }
                },
                tabControl = new SerialTabControl(),
                new IconButton
                {
                    Icon = FontAwesome.Solid.Times,
                    IconSize = new Vector2(0.6f),
                    Size = new Vector2(SerialTabControl.HEIGHT),
                    Action = () =>
                    {
                        Hide();
                    },
                    Origin = Anchor.TopRight,
                    Anchor = Anchor.TopRight
                }
            };

            handlePortsUpdated(serialConnections.PortNames.ToArray(), Array.Empty<string>());

            serialConnections.PortsUpdated += handlePortsUpdated;
            serialConnections.MessageReceived += handleMessage;
            tabControl.Current.ValueChanged += handleTabChanged;
        }

        private void handleTabChanged(ValueChangedEvent<string> e)
        {
            var tab = loadedChannels.Find(c => c.Port == e.NewValue);
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
                if (!loadedChannels.Any(c => c.Port == port))
                {
                    tabControl.AddItem(port);
                    loadedChannels.Add(new SerialChannel(port)
                    {
                        RelativeSizeAxes = Axes.Both
                    });
                }
            }
        });

        private void handleMessage(MessageInfo msg) => Schedule(() =>
        {
            var channel = loadedChannels.Find(c => c.Port == msg.Port);
            channel.AddMessage(msg);
        });

        protected override void PopIn()
        {
            this.MoveToY(0, 250, Easing.OutQuint);

            base.PopIn();
        }

        protected override void PopOut()
        {
            this.MoveToY(2, 250, Easing.In);

            base.PopOut();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            serialConnections.PortsUpdated -= handlePortsUpdated;
            serialConnections.MessageReceived -= handleMessage;
        }

        private class CloseButton : Button
        {
            private SpriteIcon icon;

            public CloseButton()
            {
                AutoSizeAxes = Axes.Both;

                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Gray
                    },
                    icon = new SpriteIcon
                    {
                        Size = new Vector2(12),
                        Icon = FontAwesome.Solid.Times,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Margin = new MarginPadding(2)
                    }
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                icon.FadeColour(Colour4.LightGray, 50, Easing.InOutQuint);

                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                icon.FadeColour(Colour4.White, 50, Easing.InOutQuint);

                base.OnHoverLost(e);
            }
        }
    }
}
