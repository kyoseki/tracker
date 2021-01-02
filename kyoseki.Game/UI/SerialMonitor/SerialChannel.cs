using System.Collections.Generic;
using System.Linq;
using kyoseki.Game.Serial;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace kyoseki.Game.UI.SerialMonitor
{
    public class SerialChannel : Container
    {
        private readonly ChannelScrollContainer scroll;

        private readonly DrawablePool<Message> messagePool = new DrawablePool<Message>(150);

        public readonly string Port;

        private readonly List<MessageInfo> messages = new List<MessageInfo>();

        private ScrollToBottomButton continueAutoscroll;

        public SerialChannel(string port)
        {
            Port = port;

            Children = new Drawable[]
            {
                messagePool,
                scroll = new ChannelScrollContainer
                {
                    ScrollbarVisible = true,
                    RelativeSizeAxes = Axes.Both
                },
                continueAutoscroll = new ScrollToBottomButton
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Alpha = 0,
                    Action = () =>
                    {
                        scroll.ResetScroll();
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
        }

        protected override void Update()
        {
            base.Update();

            if (messages.Count == 0) return;

            updateYPositions();

            var topBound = scroll.Current - 100;
            var bottomBound = scroll.Current + DrawHeight + 100;

            var firstIdx = messages.IndexOf(messages.First(m => m.ChannelYPosition >= topBound));
            var lastIdx = messages.IndexOf(messages.Last(m => m.ChannelYPosition <= bottomBound));

            var toDisplay = messages.GetRange(firstIdx, lastIdx - firstIdx + 1);

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
                scroll.Add(msg);
            }
        }

        private void updateYPositions()
        {
            float currentY = 0;

            foreach (MessageInfo msg in messages)
            {
                msg.ChannelYPosition = currentY;

                currentY += Message.HEIGHT + Message.MARGIN;
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

        private class ChannelScrollContainer : ScrollContainer<Message>
        {
            public BindableBool UserScrolling { get; private set; } = new BindableBool(false);

            public ChannelScrollContainer()
            {
                ScrollContent.AutoSizeAxes = Axes.None;
            }

            protected override ScrollbarContainer CreateScrollbar(Direction direction) => new ChannelScrollbar(direction);

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

            private class ChannelScrollbar : ScrollbarContainer
            {
                private const float dim_size = 8;

                public ChannelScrollbar(Direction direction)
                    : base(direction)
                {
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Gray
                    };
                }

                public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
                {
                    Vector2 size = new Vector2(dim_size)
                    {
                        [(int)ScrollDirection] = val
                    };
                    this.ResizeTo(size, duration, easing);
                }
            }
        }
    }
}
