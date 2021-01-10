using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;

namespace kyoseki.Game.UI.Input
{
    public class KyosekiTextBox : TextBox
    {
        public const float HEIGHT = 35;

        protected override float LeftRightPadding => HEIGHT / 2;

        protected virtual float CaretWidth => 2;

        private Colour4 backgroundFocused = KyosekiColors.TEXTBOX_FOCUSED;
        protected Colour4 BackgroundFocused
        {
            get => backgroundFocused;
            set
            {
                backgroundFocused = value;
                if (HasFocus)
                    background.Colour = value;
            }
        }

        private Colour4 backgroundUnfocused = KyosekiColors.TEXTBOX_UNFOCUSED;
        protected Colour4 BackgroundUnfocused
        {
            get => backgroundUnfocused;
            set
            {
                backgroundUnfocused = value;
                if (!HasFocus)
                    background.Colour = value;
            }
        }

        protected Colour4 BackgroundCommit { get; set; } = KyosekiColors.TEXT_SELECTED;

        protected virtual Colour4 SelectionColour => KyosekiColors.TEXT_SELECTED;

        private readonly FontUsage font = new FontUsage("Manrope");

        public new BindableBool ReadOnly = new BindableBool();

        private Box background;

        public KyosekiTextBox()
        {
            Add(background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Depth = 1,
                Colour = BackgroundUnfocused
            });

            Height = HEIGHT;
            CornerRadius = HEIGHT / 2;

            TextContainer.Height = 0.75f;
            TextFlow.Padding = new MarginPadding
            {
                Vertical = 3
            };

            ReadOnly.ValueChanged += e =>
            {
                UpdateState(e.NewValue);
            };
        }

        public new void Commit() => base.Commit();

        protected virtual void UpdateState(bool readOnly)
        {
            if (readOnly)
                KillFocus();

            var target = readOnly ? KyosekiColors.TEXTBOX_READONLY : KyosekiColors.TEXTBOX_UNFOCUSED;
            background.ClearTransforms();
            background.FadeColour(target, 100, Easing.In);

            base.ReadOnly = readOnly;
        }

        protected override Caret CreateCaret() => new BasicTextBox.BasicCaret
        {
            CaretWidth = CaretWidth,
            SelectionColour = SelectionColour
        };

        protected override SpriteText CreatePlaceholder() => new BasicTextBox.FadingPlaceholderText
        {
            Colour = Colour4.White,
            Font = font.With(weight: "Bold"),
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            X = CaretWidth
        };

        protected override Drawable GetDrawableCharacter(char c) => new BasicTextBox.FallingDownContainer
        {
            AutoSizeAxes = Axes.Both,
            Child = new SpriteText { Text = c.ToString(), Font = font.With(size: CalculatedTextSize) }
        };

        protected override void OnTextCommitted(bool textChanged)
        {
            base.OnTextCommitted(textChanged);

            background.Colour = ReleaseFocusOnCommit ? BackgroundUnfocused : BackgroundFocused;
            background.ClearTransforms();
            background.FlashColour(BackgroundCommit, 400);
        }

        protected override void OnFocusLost(FocusLostEvent e)
        {
            base.OnFocusLost(e);

            background.ClearTransforms();
            background.Colour = BackgroundFocused;
            background.FadeColour(BackgroundUnfocused, 200, Easing.OutExpo);
        }

        protected override void OnFocus(FocusEvent e)
        {
            base.OnFocus(e);

            background.ClearTransforms();
            background.Colour = BackgroundUnfocused;
            background.FadeColour(BackgroundFocused, 200, Easing.Out);
        }

        protected override void NotifyInputError()
        {
            const int shakes = 2;
            const int shake_duration = 50;
            const int shake = 5;

            var origin = LeftRightPadding;
            var sequence = new TransformSequence<Container>(TextContainer);

            for (int i = 0; i < shakes; i++)
            {
                sequence
                    .MoveToX(LeftRightPadding - shake, shake_duration, Easing.OutSine)
                    .Then()
                    .MoveToX(LeftRightPadding + shake, shake_duration / 2, Easing.InOutSine)
                    .Then();
            }

            sequence.MoveToX(LeftRightPadding, shake_duration, Easing.InSine);
        }
    }
}
