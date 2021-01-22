using System;
using System.Linq;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transforms;
using osuTK;

namespace kyoseki.Game.UI.Input
{
    public class ButtonTextBox : KyosekiTextBox
    {
        private readonly Container buttonContainer;

        private ButtonInfo[] buttons;

        public ButtonInfo[] Buttons
        {
            get => buttons;
            set
            {
                buttons = value;

                buttonContainer.Clear();
                buttons.Reverse().ForEach(AddButton);
            }
        }

        private int buttonCount => buttonContainer.Children.OfType<SideButton>().Count();

        private TransformSequence<Container> transform;

        public ButtonTextBox()
        {
            Add(buttonContainer = new Container
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X
            });
        }

        public void AddButton(ButtonInfo info)
        {
            buttonContainer.Add(new SideButton
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Size = new Vector2(HEIGHT * (buttons.Length - buttonCount), HEIGHT),
                Icon = info.Icon,
                BackgroundColour = KyosekiColors.ButtonBackground.Lighten((buttons.Length - buttonCount) * 0.25f),
                Action = info.Action,
                TooltipText = info.Tooltip
            });
        }

        protected override void Update()
        {
            base.Update();

            if (transform != null)
                return;

            transform = TextFlow.DrawWidth > DrawWidth - buttonContainer.DrawWidth - LeftRightPadding ? buttonContainer.FadeOut(100, Easing.OutQuad) : buttonContainer.FadeIn(100, Easing.In);

            transform.OnComplete(_ => transform = null);
        }

        private class SideButton : IconButton, IHasTooltip
        {
            public string TooltipText { get; set; }

            public SideButton()
            {
                Masking = true;
                CornerRadius = HEIGHT / 2f;
            }

            protected override SpriteIcon CreateIcon() => new SpriteIcon
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                X = HEIGHT / 2,
                Size = new Vector2(0.3f),
                Colour = KyosekiColors.Foreground
            };
        }
    }

    public class ButtonInfo
    {
        public readonly IconUsage Icon;

        public readonly string Tooltip;

        public readonly Action Action;

        public ButtonInfo(IconUsage icon, string tooltip, Action action = null)
        {
            Icon = icon;
            Tooltip = tooltip;
            Action = action;
        }
    }
}
