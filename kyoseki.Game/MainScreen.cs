using kyoseki.Game.Kinematics.Drawables;
using kyoseki.Game.Overlays.SerialMonitor;
using kyoseki.Game.Overlays.Skeleton;
using kyoseki.Game.Serial;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;

namespace kyoseki.Game
{
    public class MainScreen : Screen
    {
        [BackgroundDependencyLoader]
        private void load(SkeletonLinkManager skeletonLinks, SerialMonitorOverlay serial, SkeletonOverlay skeletons)
        {
            Bindable<string> port = new Bindable<string>(string.Empty);
            Bindable<string> receiverId = new Bindable<string>(string.Empty);
            Bindable<string> sensorId = new Bindable<string>(string.Empty);

            SkeletonLink link = new SkeletonLink(string.Empty, 0);
            skeletonLinks.Register(link);
            var sensorLink = link.Register("RightElbow", 0);

            void updateLink<T>(ValueChangedEvent<T> _)
            {
                if (receiverId.Value == string.Empty || sensorId.Value == string.Empty)
                {
                    return;
                }

                link.Port = port.Value;
                link.ReceiverId = int.Parse(receiverId.Value);

                link.UpdateLink("RightElbow", int.Parse(sensorId.Value));
            }

            port.ValueChanged += updateLink;
            receiverId.ValueChanged += updateLink;
            sensorId.ValueChanged += updateLink;

            InternalChildren = new Drawable[]
            {
                new DrawableSkeleton(link.Skeleton)
                {
                    RelativeSizeAxes = Axes.Both
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Direction = FillDirection.Vertical,
                    Children = new Drawable[]
                    {
                        new BasicTextBox
                        {
                            PlaceholderText = "Serial Port",
                            Current = { BindTarget = port },
                            Size = new Vector2(250, 20)
                        },
                        new NumberTextBox
                        {
                            PlaceholderText = "Receiver ID",
                            Current = { BindTarget = receiverId },
                            Size = new Vector2(250, 20)
                        },
                        new NumberTextBox
                        {
                            PlaceholderText = "Sensor ID",
                            Current = { BindTarget = sensorId },
                            Size = new Vector2(250, 20)
                        },
                        new TextButton
                        {
                            Size = new Vector2(100, 25),
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Text = "Calibrate",
                            Action = () => sensorLink.Calibrate()
                        }
                    }
                },
                new TextButton
                {
                    Size = new Vector2(100, 25),
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Text = "Open Serial",
                    Action = () => serial?.Show()
                },
                new TextButton
                {
                    Position = new Vector2(100, 0),
                    Size = new Vector2(125, 25),
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Text = "Open Skeletons",
                    Action = () => skeletons?.Show()
                }
            };
        }

        private class NumberTextBox : BasicTextBox
        {
            protected override bool CanAddCharacter(char character) => int.TryParse(character.ToString(), out int _);
        }
    }
}
