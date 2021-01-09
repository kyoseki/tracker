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
                buttons.Reverse().ForEach(b => AddButton(b));
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
                Size = new Vector2(KyosekiTextBox.HEIGHT * (buttons.Count() - buttonCount), KyosekiTextBox.HEIGHT),
                Icon = info.Icon,
                BackgroundColour = KyosekiColors.BUTTON_BACKGROUND.Lighten((buttons.Count() - buttonCount) * 0.25f),
                Action = info.Action,
                TooltipText = info.Tooltip
            });
        }

        protected override void Update()
        {
            base.Update();

            if (transform == null)
            {
                if (TextFlow.DrawWidth > DrawWidth - buttonContainer.DrawWidth - LeftRightPadding)
                    transform = buttonContainer.FadeOut(100, Easing.OutQuad);
                else
                    transform = buttonContainer.FadeIn(100, Easing.In);

                transform.OnComplete(_ => transform = null);
            }
        }

        private class SideButton : IconButton, IHasTooltip
        {
            public string TooltipText { get; set; }

            protected override Container CreateContent() =>
                new CircularContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true
                };

            protected override SpriteIcon CreateIcon() => new SpriteIcon
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                X = KyosekiTextBox.HEIGHT / 2,
                Size = new Vector2(0.3f),
                Colour = KyosekiColors.FOREGROUND
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