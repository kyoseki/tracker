using System;
using kyoseki.Game.Serial;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace kyoseki.Game.Overlays.SerialMonitor
{
    public class Message : PoolableDrawable
    {
        private const int pill_font_size = 10;
        private const int pill_width = 30;
        private const int pill_height = pill_font_size + 5;

        public const int HEIGHT = pill_height;
        public const int MARGIN = 5;

        private const int content_font_size = 12;

        private CircularContainer pillContainer;
        private Box pill;
        private SpriteText pillText;

        private TextFlowContainer textFlow;

        private MessageInfo item;

        public MessageInfo Item
        {
            get => item;
            set
            {
                item = value;

                var drawInfo = getDrawInfo(item.Direction);

                pillContainer.FadeIn(200, Easing.InQuint);
                pill.Colour = drawInfo.Colour;
                pillText.Text = drawInfo.Abbreviation;

                textFlow.Clear();
                textFlow.AddText(item.Content, t => t.Font = new FontUsage("JetbrainsMono", size: content_font_size));
            }
        }

        public Message()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            InternalChildren = new Drawable[]
            {
                pillContainer = new CircularContainer
                {
                    Alpha = 0,
                    Size = new Vector2(pill_width, pill_height),
                    Masking = true,
                    Children = new Drawable[]
                    {
                        pill = new Box
                        {
                            RelativeSizeAxes = Axes.Both
                        },
                        pillText = new SpriteText
                        {
                            Font = new FontUsage("Manrope", size: pill_font_size, weight: "Bold"),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Padding = new MarginPadding
                            {
                                Horizontal = pill_width / 2,
                                Vertical = pill_height / 2
                            }
                        }
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Padding = new MarginPadding
                    {
                        Left = pill_width + 5
                    },
                    Child = textFlow = new TextFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y
                    }
                }
            };
        }

        private MessageDrawInfo getDrawInfo(MessageDirection direction)
        {
            switch (direction)
            {
                case MessageDirection.Outgoing:
                    return new MessageDrawInfo
                    {
                        Colour = Colour4.Green,
                        Abbreviation = "TX"
                    };
                case MessageDirection.Incoming:
                    return new MessageDrawInfo
                    {
                        Colour = Colour4.Blue,
                        Abbreviation = "RX"
                    };
                default:
                    throw new ArgumentException("MessageDirection has no entry for provided value.");
            }
        }

        private class MessageDrawInfo
        {
            public ColourInfo Colour { get; set; }

            public string Abbreviation { get; set; }
        }
    }

    public enum MessageDirection
    {
        Outgoing,
        Incoming
    }
}
