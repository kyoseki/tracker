using System;
using System.Collections.Generic;
using System.Linq;
using kyoseki.Game.Serial;
using kyoseki.Game.UI;
using kyoseki.Game.UI.Buttons;
using kyoseki.Game.UI.Input;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace kyoseki.Game.Overlays.SerialMonitor
{
    public class SerialChannel : Container
    {
        private readonly ChannelScrollContainer scroll;

        private readonly DrawablePool<Message> messagePool = new DrawablePool<Message>(150);

        private readonly ISerialPort port;

        public readonly string PortName;

        private readonly List<MessageInfo> messages = new List<MessageInfo>();

        private ScrollToBottomButton continueAutoscroll;

        private ButtonTextBox textBox;

        public readonly Bindable<SerialPortState> PortState = new Bindable<SerialPortState>();

        private ButtonInfo[] portOpenButtons => new ButtonInfo[]
        {
            new ButtonInfo(FontAwesome.Solid.ArrowRight, "Send Message", () => textBox.Commit()),
            new ButtonInfo(FontAwesome.Solid.Plug, "Release Port", () => port.Release()),
            new ButtonInfo(FontAwesome.Solid.Trash, "Clear Messages", clearMessages)
        };

        private ButtonInfo[] portReconnectButtons => new ButtonInfo[]
        {
            new ButtonInfo(FontAwesome.Solid.Undo, "Reconnect", () => port.Open())
        };

        public Action<string, string> SendMessage;

        public SerialChannel(ISerialPort port)
        {
            PortName = port.Name;
            this.port = port;

            Child = new TooltipContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    messagePool,
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Bottom = KyosekiTextBox.HEIGHT },
                        Children = new Drawable[]
                        {
                            scroll = new ChannelScrollContainer
                            {
                                ScrollbarVisible = true,
                                RelativeSizeAxes = Axes.Both,
                            },
                            continueAutoscroll = new ScrollToBottomButton
                            {
                                Anchor = Anchor.BottomRight,
                                Origin = Anchor.BottomRight,
                                Alpha = 0,
                                Action = () =>
                                {
                                    scroll.ResetScroll();
                                    scroll.ScrollToEnd();
                                }
                            }
                        }
                    },
                    textBox = new ButtonTextBox
                    {
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        PlaceholderText = "Enter message to device...",
                        ReleaseFocusOnCommit = false
                    }
                }
            };

            scroll.UserScrolling.ValueChanged += e =>
            {
                if (e.NewValue)
                    continueAutoscroll.FadeIn(50, Easing.InQuint);
                else
                    continueAutoscroll.FadeOut(50, Easing.OutQuint);
            };

            PortState.ValueChanged += e => updateState(e.NewValue);
            updateState(port.State.Value);

            textBox.OnCommit += (_, __) =>
            {
                SendMessage?.Invoke(PortName, textBox.Text);
                textBox.Text = string.Empty;
            };
        }

        private void updateState(SerialPortState state) => Schedule(() =>
        {
            switch (state)
            {
                case SerialPortState.Open:
                    textBox.Buttons = portOpenButtons;
                    textBox.PlaceholderText = "Enter message to device...";
                    textBox.ReadOnly.Value = false;
                    break;
                case SerialPortState.AccessDenied:
                    textBox.Buttons = portReconnectButtons;
                    textBox.PlaceholderText = "Access to this device was denied";
                    textBox.ReadOnly.Value = true;
                    break;
                case SerialPortState.Released:
                    textBox.Buttons = portReconnectButtons;
                    textBox.PlaceholderText = "Device was released to another program";
                    textBox.ReadOnly.Value = true;
                    break;
                default:
                    textBox.Buttons = Array.Empty<ButtonInfo>();
                    textBox.PlaceholderText = "Device was disconnected";
                    textBox.ReadOnly.Value = true;
                    break;
            }
        });

        private void clearMessages()
        {
            foreach (var msg in scroll.Children)
            {
                msg.FadeOut(100, Easing.OutQuad).Expire();
            }

            messages.Clear();
        }

        protected override void Update()
        {
            base.Update();

            if (messages.Count == 0) return;

            updateYPositions();

            var topBound = scroll.Current - 100;
            var bottomBound = scroll.Current + DrawHeight + 100;

            var toDisplay = messages.Where(m => m.ChannelYPosition >= topBound && m.ChannelYPosition <= bottomBound).ToList();

            foreach (var msg in scroll.Children)
            {
                if (toDisplay.Remove(msg.Item))
                {
                    continue;
                }

                if (msg.Y + msg.DrawHeight < topBound - 100 || msg.Y > bottomBound + 100)
                {
                    msg.Expire();
                }
            }

            foreach (var item in toDisplay)
            {
                var msg = messagePool.Get(p => p.Item = item);

                msg.Y = item.ChannelYPosition;
                msg.Show();
                scroll.Add(msg);
            }
        }

        private void updateYPositions()
        {
            float currentY = 0;

            foreach (MessageInfo msg in messages)
            {
                msg.ChannelYPosition = currentY;

                var additionalLines = msg.Content.Split("\n").Count() - 1;
                var additionalHeight = Math.Max(0, additionalLines * (Message.CONTENT_FONT_SIZE + Message.CONTENT_LINE_SPACING));

                currentY += Message.HEIGHT + Message.MARGIN + additionalHeight;
            }

            scroll.ScrollContent.Height = currentY;
        }

        public void AddMessage(MessageInfo msg) => Schedule(() =>
        {
            messages.Add(msg);

            if (!scroll.UserScrolling.Value)
            {
                scroll.ScrollToEnd();
            }
        });

        private class ScrollToBottomButton : IconButton
        {
            protected override Container CreateContent() =>
                new CircularContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true
                };

            public ScrollToBottomButton()
            {
                Icon = FontAwesome.Solid.ChevronDown;
                IconSize = new Vector2(0.6f);
                Size = new Vector2(40);
            }
        }

        private class ChannelScrollContainer : KyosekiScrollContainer<Message>
        {
            public BindableBool UserScrolling { get; private set; } = new BindableBool(false);

            public ChannelScrollContainer()
            {
                ScrollContent.AutoSizeAxes = Axes.None;
            }

            protected override void OnUserScroll(float value, bool animated = true, double? distanceDecay = null)
            {
                UserScrolling.Value = true;

                base.OnUserScroll(value, animated, distanceDecay);
            }

            protected override void Update()
            {
                UserScrolling.Value |= Scrollbar.IsDragged;

                base.Update();
            }

            public void ResetScroll() => UserScrolling.Value = false;
        }
    }
}
