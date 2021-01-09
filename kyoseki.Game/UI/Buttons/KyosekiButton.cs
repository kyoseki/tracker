using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;

namespace kyoseki.Game.UI.Buttons
{
    public class KyosekiButton : Button
    {
        protected Box Hover;
        protected Drawable Background;

        protected override Container<Drawable> Content { get; }

        public ColourInfo BackgroundColour
        {
            get => Background.Colour;
            set => Background.Colour = value;
        }

        protected virtual Container CreateContent() =>
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Margin = new MarginPadding(2)
            };

        public KyosekiButton()
        {
            AddInternal(CreateContent().WithChildren(new Drawable[]
            {
                Background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = KyosekiColors.BUTTON_BACKGROUND
                },
                Content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Padding = new MarginPadding(2)
                },
                Hover = new Box
                {
                    Alpha = 0,
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.White.Opacity(0.1f)
                }
            }));
        }

        protected override bool OnHover(HoverEvent e)
        {
            Hover.FadeIn(200, Easing.In);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            Hover.FadeOut(200, Easing.Out);
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            Background.FlashColour(KyosekiColors.BUTTON_SELECTED, 200);
            return base.OnClick(e);
        }
    }
}
