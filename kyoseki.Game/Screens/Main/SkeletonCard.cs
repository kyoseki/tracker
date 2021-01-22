using System;
using kyoseki.Game.Kinematics.Drawables;
using kyoseki.Game.Serial;
using kyoseki.Game.UI;
using kyoseki.Game.UI.Buttons;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace kyoseki.Game.Screens.Main
{
    public class SkeletonCard : KyosekiButton
    {
        private const int width = 280;
        private const int height = 480;

        private const int skeleton_height = 340;

        private const int padding_side = 20;
        private const int padding_bottom = 60;

        private SkeletonLink skeletonLink;

        public Action<string> OpenPort;
        public Action<SkeletonLink> OpenSkeleton;

        public SkeletonLink SkeletonLink
        {
            get => skeletonLink;
            set
            {
                skeletonLink = value;

                drawableSkeleton.Skeleton = value.Skeleton;
            }
        }

        private readonly DrawableSkeleton drawableSkeleton;
        private readonly SpriteText idText;
        private readonly ComPortButton button;

        public SkeletonCard()
        {
            Width = width;
            Height = height;
            Masking = true;
            CornerRadius = 12;

            FlashColour = KyosekiColors.ButtonSelected.Darken(0.5f);

            Hover.Colour = Colour4.White.Opacity(0.01f);

            Children = new Drawable[]
            {
                drawableSkeleton = new SkeletonCardSkeleton
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.X,
                    Height = 340
                },
                idText = new SpriteText
                {
                    RelativeSizeAxes = Axes.X,
                    Width = 0.5f,
                    Font = KyosekiFont.Bold.With(size: 36),
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.CentreLeft,
                    Padding = new MarginPadding { Left = padding_side, Bottom = padding_bottom }
                },
                button = new ComPortButton
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.CentreRight,
                    Margin = new MarginPadding { Right = padding_side, Bottom = padding_bottom },
                    Action = () => OpenPort?.Invoke(SkeletonLink.Port)
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            OpenSkeleton?.Invoke(SkeletonLink);

            return base.OnClick(e);
        }

        private int? receiverId;
        private string port;

        protected override void Update()
        {
            base.Update();

            if (receiverId != SkeletonLink.ReceiverId)
            {
                receiverId = SkeletonLink.ReceiverId;
                idText.Text = $"#{receiverId}";
            }

            if (port != SkeletonLink.Port)
            {
                port = SkeletonLink.Port;
                button.Text = port;
            }
        }

        private class ComPortButton : TextButton
        {
            private const int button_height = 28;

            public ComPortButton()
            {
                Size = new Vector2(110, button_height);
                Masking = true;
                CornerRadius = button_height / 2f;
                BackgroundColour = Colour4.FromHex("46496A");
                Font = KyosekiFont.Bold.With(size: 24);
            }
        }

        private class SkeletonCardSkeleton : DrawableSkeleton
        {
            protected override float SkeletonDrawScale => 3.75f * ScreenSpaceDrawQuad.Height / skeleton_height;

            public override bool HandleNonPositionalInput => false;
        }
    }
}
