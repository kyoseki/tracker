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

        public SkeletonLink Link;

        private KyosekiDropdown<string> portDropdown;
        private KyosekiDropdown<int> receiverDropdown;

        private FillFlowContainer<SensorLinkViewButton> sensorFlow;

        private TextButton calibrateAllButton;

        private EditorDrawableSkeleton drawableSkeleton;

        private SensorPopout sensorPopout;

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
                        drawableSkeleton = new EditorDrawableSkeleton
                        {
                            RelativeSizeAxes = Axes.Both,
                            BoneClicked = bone =>
                            {
                                if (!sensorPopout.Linking)
                                    return;

                                Link?.Register(selectedSensor.Link.SensorId, bone.Name, true);

                                sensorPopout.UpdateState(true);
                            }
                        },
                        calibrateAllButton = new TextButton
                        {
                            Size = new Vector2(182, 25),
                            Text = "Calibrate All",
                            CornerRadius = 12.5f,
                            Masking = true,
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre
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
                sensorPopout = new SensorPopout
                {
                    RelativeSizeAxes = Axes.Y,
                    RelativePositionAxes = Axes.Y,
                    Width = sidebar_width,
                    X = -sidebar_width,
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Height = 0.4f,
                    Deselect = deselect
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
                        Padding = new MarginPadding { Top = TITLE_HEIGHT },
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
                        Height = TITLE_HEIGHT,
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
                                Font = KyosekiFont.Bold.With(size: TITLE_HEIGHT * 0.6f),
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

            sensorPopout.SelectedOrientation.ValueChanged += e =>
            {
                selectedSensor.Link.MountOrientation = e.NewValue;
            };

            serialConnections.PortsUpdated += handlePortsUpdated;

            SetLink(skeletonLinks.SkeletonLinks.FirstOrDefault() ?? new SkeletonLink());
        }

        public void SetLink(SkeletonLink link, bool register = false)
        {
            if (Link != null)
                Link.SensorsUpdated -= handleSensorUpdate;

            if (register)
                skeletonLinks.Register(link, serialConnections.PortNames.First());

            Link = link;

            portDropdown.Current.Value = string.IsNullOrEmpty(link.Port) ? portDropdown.Items.ElementAt(0) : link.Port;

            link.SensorsUpdated += handleSensorUpdate;
            drawableSkeleton.Skeleton = link.Skeleton;
            calibrateAllButton.Action = link.CalibrateAll;

            handleSensorUpdate();
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

            deselect();

            if (sensor != null)
            {
                sensor.Selected = true;

                selectedSensor = sensor;

                sensorPopout.SelectedSensor = sensor.Link;
                sensorPopout.Show();
            }
            else
            {
                sensorPopout.Hide();
            }
        }

        private void deselect()
        {
            if (selectedSensor != null)
            {
                selectedSensor.Selected = false;
                selectedSensor = null;
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
            protected override float SkeletonDrawScale => 4 * ScreenSpaceDrawQuad.Height / skeleton_size;
        }
    }
}
