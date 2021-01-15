using kyoseki.Game.Kinematics.Drawables;
using kyoseki.Game.MathUtils;
using kyoseki.Game.Serial;
using kyoseki.Game.UI;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace kyoseki.Game.Overlays.Skeleton
{
    public class SensorLinkView : CompositeDrawable
    {
        private const int height = 56;
        private const int width = 320;
        private const int corner_radius = 7;

        private const int orientation_text_size = height / 3 - 2;

        private readonly SensorLink link;

        private readonly SpriteText boneName;

        public SensorLinkView(SensorLink link)
        {
            this.link = link;

            SpriteText xText;
            SpriteText yText;
            SpriteText zText;

            CornerRadius = corner_radius;
            Masking = true;
            Width = width;
            Height = height;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = KyosekiColors.Background.Lighten(0.5f)
                },
                new Container
                {
                    Size = new Vector2(height - 5),
                    Padding = new MarginPadding(5),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Child = new SensorView
                    {
                        RelativeSizeAxes = Axes.Both,
                        Link = link
                    }
                },
                new Container
                {
                    Width = width - height,
                    Height = height,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    CornerRadius = corner_radius,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = KyosekiColors.ButtonBackground
                        },
                        new SpriteText
                        {
                            Font = KyosekiFont.Bold.With(size: 20),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.Centre,
                            Padding = new MarginPadding { Left = 50 },
                            Text = link.SensorId.ToString()
                        },
                        boneName = new SpriteText
                        {
                            Font = KyosekiFont.GetFont(size: 18),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Width = 135,
                            X = -10,
                            Truncate = true
                        },
                        new Container
                        {
                            Width = 56,
                            RelativeSizeAxes = Axes.Y,
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Padding = new MarginPadding { Vertical = 5, Right = 5 },
                            Children = new Drawable[]
                            {
                                xText = new SpriteText
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    Font = KyosekiFont.GetFont(size: orientation_text_size)
                                },
                                yText = new SpriteText
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Font = KyosekiFont.GetFont(size: orientation_text_size)
                                },
                                zText = new SpriteText
                                {
                                    RelativeSizeAxes = Axes.X,
                                    Anchor = Anchor.BottomCentre,
                                    Origin = Anchor.BottomCentre,
                                    Font = KyosekiFont.GetFont(size: orientation_text_size)
                                }
                            }
                        }
                    }
                }
            };

            link.CalibratedOrientation.ValueChanged += e =>
            {
                var euler = e.NewValue.ToEuler();

                xText.Text = $"X: {(int)euler.x}°";
                yText.Text = $"Y: {(int)euler.y}°";
                zText.Text = $"Z: {(int)euler.z}°";
            };
        }

        // TODO: implement better if necessary
        protected override void Update()
        {
            base.Update();

            if (link.BoneName != boneName.Text)
                boneName.Text = link.BoneName;
        }
    }
}
