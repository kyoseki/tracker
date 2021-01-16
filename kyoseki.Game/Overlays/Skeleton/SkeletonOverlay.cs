using System;
using System.Linq;
using kyoseki.Game.Kinematics;
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

        protected override string Title => "Skeleton Editor";

        public readonly SkeletonLink Link = new SkeletonLink();

        private KyosekiDropdown<string> portDropdown;
        private KyosekiDropdown<int> receiverDropdown;
        private KyosekiDropdown<MountOrientation> mountDropdown;

        private SpriteText boneText;

        private Bone currentBone;

        private FillFlowContainer sensorFlow;

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
                        boneText = new SpriteText
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            Text = "Select a bone",
                            Font = KyosekiFont.Bold.With(size: 24)
                        },
                        new EditorDrawableSkeleton(Link.Skeleton)
                        {
                            RelativeSizeAxes = Axes.Both,
                            BoneClicked = bone =>
                            {
                                currentBone = bone;
                                boneText.Text = bone.Name;

                                var link = Link.Get(bone.Name, true);
                                mountDropdown.Current.Value = link?.MountOrientation ?? MountOrientation.ZUpYForward;
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
                        mountDropdown = new KyosekiDropdown<MountOrientation>
                        {
                            Width = 150,
                            X = 250,
                            Depth = 2,
                            Items = Enum.GetValues(typeof(MountOrientation)).Cast<MountOrientation>()
                        }
                    }
                }
            });

            AddInternal(new Container
            {
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Width = SensorLinkView.WIDTH + 20,
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
                        Child = sensorFlow = new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y
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
                if (currentBone == null)
                    return;

                var link = Link.Get(currentBone.Name, true);

                if (link != null)
                    link.MountOrientation = e.NewValue;
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
                        Action = handleSensorClick
                    });
                }
            }
        });

        private void handleSensorClick(SensorLink link)
        {
            if (currentBone == null)
                return;

            Link.Register(link.SensorId, currentBone.Name, true);
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
            public Action<SensorLink> Action;

            public SensorLinkViewButton(SensorLink link)
                : base(link)
            {
            }

            protected override bool OnClick(ClickEvent e)
            {
                Action?.Invoke(Link);

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
