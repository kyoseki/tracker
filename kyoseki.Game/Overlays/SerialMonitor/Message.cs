using System;
using kyoseki.Game.Serial;
using kyoseki.Game.UI;
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

        public const int CONTENT_FONT_SIZE = 12;

        public const int CONTENT_LINE_SPACING = 0;

        private readonly Box pill;
        private readonly SpriteText pillText;

        private readonly TextFlowContainer textFlow;

        private MessageInfo item;

        public MessageInfo Item
        {
            get => item;
            set
            {
                item = value;

                var (colour, abbreviation) = getDrawInfo(item.Direction);

                pill.Colour = colour;
                pillText.Text = abbreviation;

                textFlow.Clear();
                var lines = item.Content.Split("\n");

                foreach (var line in lines)
                {
                    textFlow.AddText(new SpriteText
                    {
                        Truncate = true,
                        Text = line,
                        RelativeSizeAxes = Axes.X,
                        Font = KyosekiFont.Mono.With(size: CONTENT_FONT_SIZE)
                    });
                }
            }
        }

        public Message()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            InternalChildren = new Drawable[]
            {
                new CircularContainer
                {
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
                            Font = KyosekiFont.Bold.With(size: pill_font_size),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
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
                        AutoSizeAxes = Axes.Y,
                        LineSpacing = CONTENT_LINE_SPACING
                    }
                }
            };
        }

        private (ColourInfo, string) getDrawInfo(MessageDirection direction)
        {
            switch (direction)
            {
                case MessageDirection.Outgoing:
                    return (Colour4.Green, "TX");

                case MessageDirection.Incoming:
                    return (Colour4.Blue, "RX");

                default:
                    throw new ArgumentException("MessageDirection has no entry for provided value.");
            }
        }
    }

    public enum MessageDirection
    {
        Outgoing,
        Incoming
    }
}
