using System;
using System.Linq;
using kyoseki.Game.Kinematics.Drawables;
using kyoseki.Game.Serial;
using kyoseki.Game.UI;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace kyoseki.Game.Overlays.Skeleton
{
    public class SkeletonOverlay : SlideInOverlay
    {
        [Resolved]
        private ConnectionManager serialConnections { get; set; }

        [Resolved]
        private SkeletonLinkManager skeletonLinks { get; set; }

        private const int skeleton_padding = 56;
        private const int skeleton_size = 348;

        private const int sidebar_width = SensorLinkView.WIDTH + 20;

        protected override string Title => "Skeleton Editor";

        public readonly SkeletonLink Link = new SkeletonLink();

        private KyosekiDropdown<string> portDropdown;
        private KyosekiDropdown<int> receiverDropdown;
        private KyosekiDropdown<MountOrientation> mountDropdown;

        private FillFlowContainer<SensorLinkViewButton> sensorFlow;

        private Container sensorPopout;
        private SpriteText selectedSensorText;
        private TextButton linkButton;

        private bool linking;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddRange(new Drawable[]
            {
                new Container
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Padding = new MarginPadding(skeleton_padding),
                    Size = new Vector2(2 * skeleton_padding + skeleton_size),
                    Children = new Drawable[]
                    {
                        new EditorDrawableSkeleton(Link.Skeleton)
                        {
                            RelativeSizeAxes = Axes.Both,
                            BoneClicked = bone =>
                            {
                                if (!linking)
                                    return;

                                linkButton.Text = bone.Name;

                                Link.Register(selectedSensor.Link.SensorId, bone.Name, true);
                            }
                        },
                        new TextButton
                        {
                            Size = new Vector2(182, 25),
                            Text = "Calibrate All",
                            CornerRadius = 12.5f,
                            Masking = true,
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            Action = Link.CalibrateAll
                        }
                    }
                },
                new Container
                {
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Padding = new MarginPadding(40),
                    Children = new Drawable[]
                    {
                        portDropdown = new KyosekiDropdown<string>
                        {
                            Width = 200,
                            Items = serialConnections.PortNames.ToArray()
                        },
                        receiverDropdown = new KyosekiDropdown<int>
                        {
                            Width = 150,
                            Y = KyosekiDropdown<string>.KyosekiDropdownHeader.HEIGHT + 2,
                            Depth = 1
                        },
                    }
                },
                sensorPopout = new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    RelativePositionAxes = Axes.Y,
                    Width = sidebar_width,
                    X = -sidebar_width,
                    Height = 0.4f,
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Y = 1,
                    Children = new Drawable[]
                    {
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
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Top = SlideInOverlay.TITLE_HEIGHT },
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = KyosekiColors.Background.Darken(0.3f)
                                },
                                new KyosekiScrollContainer
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
                                                Action = () =>
                                                {
                                                    if (!linking)
                                                    {
                                                        linkButton.Text = "Click a bone to link";
                                                        linking = true;
                                                    }
                                                    else
                                                    {
                                                        linkButton.Text = "Click to link";
                                                        linking = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });

            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Width = sidebar_width,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = KyosekiColors.Background
                    },
                    new KyosekiScrollContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Top = SlideInOverlay.TITLE_HEIGHT },
                        Child = sensorFlow = new FillFlowContainer<SensorLinkViewButton>
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Margin = new MarginPadding { Top = 10 }
                        }
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
                                Colour = KyosekiColors.Background.Darken(0.7f)
                            },
                            new SpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Font = KyosekiFont.Bold.With(size: SlideInOverlay.TITLE_HEIGHT * 0.6f),
                                Text = "Sensors",
                                Padding = new MarginPadding { Left = 15 }
                            }
                        }
                    }
                }
            });

            portDropdown.Current.ValueChanged += e =>
            {
                skeletonLinks.Register(Link, e.NewValue);
                receiverDropdown.ItemSource = skeletonLinks.GetPort(e.NewValue).ReceiverIds;

                handleSensorUpdate();
            };

            receiverDropdown.Current.ValueChanged += e =>
            {
                Link.ReceiverId = e.NewValue;

                handleSensorUpdate();
            };

            mountDropdown.Current.ValueChanged += e =>
            {
                selectedSensor.Link.MountOrientation = e.NewValue;
            };

            serialConnections.PortsUpdated += handlePortsUpdated;

            Link.SensorsUpdated += handleSensorUpdate;
        }

        private void handleSensorUpdate() => Schedule(() =>
        {
            sensorFlow.Clear();

            lock (Link.Sensors)
            {
                foreach (var sensor in Link.Sensors)
                {
                    sensorFlow.Add(new SensorLinkViewButton(sensor)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Action = select
                    });
                }
            }
        });

        private SensorLinkViewButton selectedSensor;

        private void select(SensorLinkViewButton sensor)
        {
            if (sensor == selectedSensor)
            {
                select(null);

                return;
            }

            if (selectedSensor != null)
                selectedSensor.Selected = false;

            if (sensor != null)
            {
                sensor.Selected = true;

                selectedSensor = sensor;

                selectedSensorText.Text = $"Sensor ID {sensor.Link.SensorId}";
                sensorPopout.MoveToY(0, 150, Easing.OutExpo);
                linkButton.Text = string.IsNullOrEmpty(sensor.Link.BoneName) ? "Click to link" : sensor.Link.BoneName;
                mountDropdown.Current.Value = sensor.Link.MountOrientation;
            }
            else
            {
                selectedSensor = null;

                sensorPopout.MoveToY(1, 150, Easing.In);
            }
        }

        private void handlePortsUpdated(string[] added, string[] removed) => Schedule(() =>
        {
            portDropdown.Items = serialConnections.PortNames.ToArray();
        });

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            serialConnections.PortsUpdated -= handlePortsUpdated;
        }

        private class SensorLinkViewButton : SensorLinkView
        {
            public new SensorLink Link => base.Link;

            public Action<SensorLinkViewButton> Action;

            private bool selected;

            public bool Selected
            {
                get => selected;
                set
                {
                    selected = value;
                    BorderThickness = selected ? 5 : 0;
                }
            }

            public SensorLinkViewButton(SensorLink link)
                : base(link)
            {
                BorderColour = KyosekiColors.ButtonSelected;
            }

            protected override bool OnClick(ClickEvent e)
            {
                Action?.Invoke(this);

                return true;
            }
        }

        private class EditorDrawableSkeleton : DrawableSkeleton
        {
            public override float SkeletonDrawScale => 4 * ScreenSpaceDrawQuad.Height / skeleton_size;

            public EditorDrawableSkeleton(kyoseki.Game.Kinematics.Skeleton skeleton)
                : base(skeleton)
            {
            }
        }
    }
}
