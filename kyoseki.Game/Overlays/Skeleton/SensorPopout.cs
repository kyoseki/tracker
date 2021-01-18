using System;
using System.Linq;
using kyoseki.Game.Serial;
using kyoseki.Game.UI;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace kyoseki.Game.Overlays.Skeleton
{
    public class SensorPopout : SlideInOverlay
    {
        public bool Linking { get; private set; }

        private SensorLink selectedSensor;

        public SensorLink SelectedSensor
        {
            get => selectedSensor;
            set
            {
                selectedSensor = value;

                selectedSensorText.Text = $"Sensor ID {value.SensorId}";
                SelectedOrientation.Value = value.MountOrientation;

                Linking = false;

                UpdateState();
            }
        }

        public Action Deselect;

        public Bindable<MountOrientation> SelectedOrientation => mountDropdown.Current;

        private readonly SpriteText selectedSensorText;
        private readonly KyosekiDropdown<MountOrientation> mountDropdown;
        private readonly TextButton linkButton;

        public SensorPopout()
            : base(false)
        {
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = KyosekiColors.Background.Darken(0.3f)
                },
                new Container
                {
                    Padding = new MarginPadding { Top = SlideInOverlay.TITLE_HEIGHT },
                    RelativeSizeAxes = Axes.Both,
                    Child = new KyosekiScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Margin = new MarginPadding(5),
                            Spacing = new Vector2(5),
                            Children = new Drawable[]
                            {
                                mountDropdown = new KyosekiDropdown<MountOrientation>
                                {
                                    Width = 150,
                                    Items = Enum.GetValues(typeof(MountOrientation)).Cast<MountOrientation>()
                                },
                                linkButton = new TextButton
                                {
                                    Size = new Vector2(250, 20),
                                    CornerRadius = 5,
                                    Masking = true,
                                    Action = toggleLinking
                                }
                            }
                        }
                    },
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = SlideInOverlay.TITLE_HEIGHT,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = KyosekiColors.Background.Darken(0.6f)
                        },
                        selectedSensorText = new SpriteText
                        {
                            Font = KyosekiFont.Bold.With(size: 24),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Padding = new MarginPadding { Left = 15 }
                        }
                    }
                }
            };
        }

        protected override void PopOut()
        {
            base.PopOut();

            Deselect?.Invoke();
        }

        public void UpdateState(bool stopLinking = false)
        {
            if (stopLinking)
                Linking = false;

            if (!Linking && string.IsNullOrEmpty(selectedSensor?.BoneName))
                linkButton.Text = "Click to link";
            else if (Linking)
                linkButton.Text = "Click a bone to link";
            else
                linkButton.Text = $"Linked to {selectedSensor.BoneName}";
        }

        private void toggleLinking()
        {
            Linking = !Linking;

            UpdateState();
        }
    }
}
