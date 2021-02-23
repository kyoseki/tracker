using System;
using kyoseki.Game.UI;
using kyoseki.UI.Components.Buttons;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace kyoseki.Game.Screens.Main
{
    public class Toolbar : CompositeDrawable
    {
        public const int HEIGHT = 45;

        private const float logo_scale = 0.15f;

        public Action AddSkeleton;

        public Action OpenSerial;

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Height = HEIGHT;

            var logo = textures.Get("logo");

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = KyosekiColors.ButtonBackground
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = 10 },
                    Spacing = new Vector2(15),
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        new Sprite
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Size = logo.Size,
                            Scale = new Vector2(logo_scale),
                            Texture = logo
                        },
                        new SpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = "Skeletons",
                            Font = KyosekiFont.Bold.With(size: 30)
                        },
                        new ToolbarButton
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            BackgroundColour = KyosekiColors.ButtonBackground.Lighten(0.5f),
                            CornerRadius = 15,
                            Masking = true,
                            Size = new Vector2(80, 30),
                            Icon = FontAwesome.Solid.Plus,
                            Text = "Add",
                            Action = AddSkeleton,
                            FontSize = 18
                        }
                    }
                },
                new ToolbarButton
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    BackgroundColour = KyosekiColors.ButtonBackground.Darken(0.5f),
                    RelativeSizeAxes = Axes.Y,
                    Width = 100,
                    Icon = FontAwesome.Solid.Terminal,
                    Text = "Serial",
                    FontSize = 22,
                    Action = OpenSerial
                }
            };
        }

        private class ToolbarButton : KyosekiButton
        {
            public IconUsage Icon
            {
                get => spriteIcon.Icon;
                set => spriteIcon.Icon = value;
            }

            public string Text
            {
                get => spriteText.Text;
                set => spriteText.Text = value;
            }

            private int fontSize = 15;

            public int FontSize
            {
                get => fontSize;
                set
                {
                    spriteIcon.Size = new Vector2(value - 5);
                    spriteText.Font = KyosekiFont.GetFont(size: value);

                    fontSize = value;
                }
            }

            private readonly SpriteIcon spriteIcon;
            private readonly SpriteText spriteText;

            public ToolbarButton()
            {
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        spriteIcon = new SpriteIcon
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Colour = KyosekiColors.Foreground
                        },
                        spriteText = new SpriteText
                        {
                            Truncate = true,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Colour = KyosekiColors.Foreground
                        }
                    }
                };

                DisableBackgroundTheming = true;
            }
        }
    }
}
