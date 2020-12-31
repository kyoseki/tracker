using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace kyoseki.Game.UI.SerialMonitor
{
    public class Message : CompositeDrawable
    {
        private const int pill_font_size = 10;
        private const int pill_width = 30;
        private const int pill_height = pill_font_size + 5;

        private const int content_font_size = 12;

        public readonly MessageDirection Direction;

        public readonly string Content;

        private MessageDrawInfo drawInfo => getDrawInfo(Direction);

        public Message(MessageDirection direction, string content)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            Direction = direction;
            Content = content;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new CircularContainer
                {
                    Size = new Vector2(pill_width, pill_height),
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = drawInfo.Colour
                        },
                        new SpriteText
                        {
                            Font = new FontUsage(size: pill_font_size),
                            Text = drawInfo.Abbreviation,
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
                    Child = new SpriteText
                    {
                        Font = new FontUsage(size: content_font_size),
                        Text = Content
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
