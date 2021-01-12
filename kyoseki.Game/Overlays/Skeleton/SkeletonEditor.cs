using kyoseki.Game.Kinematics;
using kyoseki.Game.Kinematics.Drawables;
using kyoseki.Game.Serial;
using kyoseki.Game.UI;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace kyoseki.Game.Overlays.Skeleton
{
    public class SkeletonEditor : Container
    {
        private const int skeleton_width = 384;

        public readonly SkeletonLink Link = new SkeletonLink();

        private BasicTextBox portInput;
        private NumberTextBox receiverIdInput;

        private SpriteText boneText;
        private NumberTextBox sensorIdInput;

        private Bone currentBone;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new DrawableSkeleton(Link.Skeleton)
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = skeleton_width,
                    BoneClicked = bone =>
                    {
                        currentBone = bone;
                        boneText.Text = bone.Name;

                        sensorIdInput.Text = Link.Get(bone.Name, true)?.SensorId.ToString() ?? string.Empty;
                        sensorIdInput.ReadOnly = false;
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = skeleton_width },
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Direction = FillDirection.Vertical,
                            Children = new Drawable[]
                            {
                                portInput = new BasicTextBox
                                {
                                    PlaceholderText = "Serial Port",
                                    Size = new Vector2(250, 20),
                                    CommitOnFocusLost = true
                                },
                                receiverIdInput = new NumberTextBox
                                {
                                    PlaceholderText = "Receiver ID",
                                    Size = new Vector2(250, 20),
                                    CommitOnFocusLost = true
                                },
                                boneText = new SpriteText
                                {
                                    Text = "Select a bone",
                                    Font = KyosekiFont.Bold.With(size: 24)
                                },
                                sensorIdInput = new NumberTextBox
                                {
                                    PlaceholderText = "Sensor ID",
                                    Size = new Vector2(250, 20),
                                    CommitOnFocusLost = true,
                                    ReadOnly = true
                                },
                                new TextButton
                                {
                                    Text = "Calibrate All",
                                    Size = new Vector2(250, 20),
                                    Action = () =>
                                    {
                                        Link.CalibrateAll();
                                    }
                                }
                            }
                        }
                    }
                }
            };

            portInput.OnCommit += (sender, newText) =>
            {
                Link.Port = sender.Text;
            };

            receiverIdInput.OnCommit += (sender, newText) =>
            {
                if (sender.Text == string.Empty) return;

                Link.ReceiverId = int.Parse(sender.Text);
            };

            sensorIdInput.OnCommit += (sender, newText) =>
            {
                if (sender.Text == string.Empty) return;

                var id = int.Parse(sender.Text);
                if (currentBone == null)
                    return;

                var existing = Link.Get(currentBone.Name, true) ?? Link.Get(id, true);

                if (existing == null)
                {
                    Link.Register(currentBone.Name, id);
                }
                else
                {
                    Link.UpdateLink(currentBone.Name, id);
                }
            };
        }

        private class NumberTextBox : BasicTextBox
        {
            protected override bool CanAddCharacter(char character) => char.IsNumber(character);
        }
    }
}
