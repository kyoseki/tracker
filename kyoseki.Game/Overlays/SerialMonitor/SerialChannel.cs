using System;
using kyoseki.Game.Serial;
using kyoseki.Game.UI.Buttons;
using kyoseki.Game.UI.Input;
using kyoseki.Game.UI.Pooling;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace kyoseki.Game.Overlays.SerialMonitor
{
    public class SerialChannel : Container
    {
        private readonly ChannelScrollContainer scroll;

        private const int scroll_to_bottom_size = 40;

        private readonly ISerialPort port;

        public string PortName { get; set; }

        private readonly ButtonTextBox textBox;

        public readonly Bindable<SerialPortState> PortState = new Bindable<SerialPortState>();

        private ButtonInfo[] portOpenButtons => new[]
        {
            new ButtonInfo(FontAwesome.Solid.ArrowRight, "Send Message", () => textBox.Commit()),
            new ButtonInfo(FontAwesome.Solid.Plug, "Release Port", () => port?.Release()),
            new ButtonInfo(FontAwesome.Solid.Trash, "Clear Messages", clearMessages)
        };

        private ButtonInfo[] portReconnectButtons => new[]
        {
            new ButtonInfo(FontAwesome.Solid.Undo, "Reconnect", () => port?.Open())
        };

        public Action<string, string> SendMessage;

        public SerialChannel(ISerialPort port)
            : this()
        {
            this.port = port;
            PortName = port.Name;
        }

        public SerialChannel()
        {
            IconButton continueAutoscroll;

            Child = new TooltipContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Bottom = KyosekiTextBox.HEIGHT },
                        Children = new Drawable[]
                        {
                            scroll = new ChannelScrollContainer
                            {
                                ScrollbarVisible = true,
                                RelativeSizeAxes = Axes.Both
                            },
                            continueAutoscroll = new IconButton
                            {
                                Anchor = Anchor.BottomRight,
                                Origin = Anchor.BottomRight,
                                Icon = FontAwesome.Solid.ChevronDown,
                                IconSize = new Vector2(0.6f),
                                Size = new Vector2(scroll_to_bottom_size),
                                CornerRadius = scroll_to_bottom_size / 2f,
                                Masking = true,
                                Action = () =>
                                {
                                    scroll.ResetScroll();
                                    scroll.ScrollToEnd();
                                },
                                Alpha = 0
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

            if (port != null)
            {
                updateState(port.State.Value);
            }

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

        private void clearMessages() =>
            scroll.Clear();

        public void AddMessage(MessageInfo msg) => Schedule(() =>
        {
            scroll.Add(new ChannelItem(msg));

            if (!scroll.UserScrolling.Value)
            {
                scroll.ScrollToEnd();
            }
        });

        private class ChannelItem : PoolableScrollItem
        {
            private const int margin = 5;

            public readonly MessageInfo Info;

            public override float Height =>
                Math.Max(Math.Max(1, Info.Content.Split("\n").Length) * (Message.CONTENT_FONT_SIZE + Message.CONTENT_LINE_SPACING), Message.HEIGHT) + margin;

            public ChannelItem(MessageInfo info)
            {
                Info = info;
            }

            public override DrawablePoolableScrollItem CreateDrawable() =>
                new DrawableChannelItem { Item = this };
        }

        private class DrawableChannelItem : DrawablePoolableScrollItem
        {
            private readonly Message message;

            private ChannelItem item => (ChannelItem)Item;

            public DrawableChannelItem()
            {
                RelativeSizeAxes = Axes.X;

                AddInternal(message = new Message());
            }

            protected override void UpdateItem()
            {
                message.Item = item.Info;
            }
        }

        private class ChannelScrollContainer : PoolingScrollContainer<DrawableChannelItem>
        {
            public BindableBool UserScrolling { get; } = new BindableBool();

            protected override void OnUserScroll(float value, bool animated = true, double? distanceDecay = null)
            {
                UserScrolling.Value = true;

                base.OnUserScroll(value, animated, distanceDecay);
            }

            protected override void Update()
            {
                base.Update();

                UserScrolling.Value |= Scrollbar.IsDragged;
            }

            public void ResetScroll() => UserScrolling.Value = false;
        }
    }
}
