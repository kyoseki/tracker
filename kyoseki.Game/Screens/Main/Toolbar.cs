using System;
using kyoseki.UI.Components.Buttons;
using kyoseki.UI.Components.Theming;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using KyosekiTheme = kyoseki.Game.UI.KyosekiTheme;

namespace kyoseki.Game.Screens.Main
{
    public class Toolbar : CompositeDrawable, IHasNestedThemeComponents
    {
        public const int HEIGHT = 45;

        private const float logo_scale = 0.15f;

        public Action AddSkeleton;

        public Action OpenSerial;

        [Themeable(nameof(UITheme.BackgroundColour), Lightness = 0.4f)]
        protected ColourInfo BackgroundColour
        {
            get => background.Colour;
            set => background.Colour = value;
        }

        [Themeable(nameof(UITheme.ForegroundColour))]
        protected ColourInfo ForegroundColour
        {
            get => title.Colour;
            set => title.Colour = value;
        }

        [Themeable(nameof(UITheme.DefaultFont))]
        protected FontUsage Font
        {
            get => title.Font;
            set => title.Font = value.With(weight: "Bold", size: 30);
        }

        private Box background;

        private SpriteText title;

        private ToolbarButton addButton;
        private ToolbarButton serialButton;

        [BackgroundDependencyLoader(true)]
        private void load(ThemeContainer themeContainer, TextureStore textures)
        {
            Height = HEIGHT;

            var logo = textures.Get("logo");

            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both
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
                        title = new SpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = "Skeletons"
                        },
                        addButton = new ToolbarButton
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
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
                serialButton = new ToolbarButton
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    RelativeSizeAxes = Axes.Y,
                    Width = 100,
                    Icon = FontAwesome.Solid.Terminal,
                    Text = "Serial",
                    FontSize = 22,
                    Action = OpenSerial
                }
            };

            if (themeContainer != null)
                themeContainer.Register(this);
            else
                this.ApplyTheme(new KyosekiTheme());
        }

        public void ApplyThemeToChildren(UITheme theme, bool fade)
        {
            addButton.ApplyTheme(theme, fade);
            serialButton.ApplyTheme(theme, fade);
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
                    fontSize = value;

                    updateFont();
                }
            }

            private FontUsage font;

            [Themeable(nameof(UITheme.DefaultFont))]
            protected FontUsage Font
            {
                get => font;
                set
                {
                    font = value;
                    updateFont();
                }
            }

            [Themeable(nameof(UITheme.ForegroundColour))]
            protected ColourInfo ForegroundColour
            {
                get => spriteIcon.Colour;
                set
                {
                    spriteIcon.Colour = value;
                    spriteText.Colour = value;
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
                            Origin = Anchor.Centre
                        },
                        spriteText = new SpriteText
                        {
                            Truncate = true,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        }
                    }
                };
            }

            private void updateFont()
            {
                spriteText.Font = Font.With(size: fontSize);
                spriteIcon.Size = new Vector2(fontSize - 5);
            }
        }
    }
}
