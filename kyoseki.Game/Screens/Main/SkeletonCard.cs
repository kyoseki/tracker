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
        public Action<SkeletonLink> RemoveSkeleton;

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
        private readonly ComPortButton portButton;
        private readonly IconButton removeButton;

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
                portButton = new ComPortButton
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.CentreRight,
                    Margin = new MarginPadding { Right = padding_side, Bottom = padding_bottom },
                    Action = () => OpenPort?.Invoke(SkeletonLink.Port)
                },
                removeButton = new IconButton
                {
                    Anchor = Anchor.TopRight,
                    Origin = Anchor.TopRight,
                    Icon = FontAwesome.Solid.Times,
                    IconColour = KyosekiColors.Background.Lighten(3),
                    Size = new Vector2(25),
                    IconSize = new Vector2(0.5f),
                    CornerRadius = 8,
                    Masking = true,
                    Alpha = 0,
                    ConsumeHover = false,
                    Action = () => RemoveSkeleton?.Invoke(SkeletonLink)
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            OpenSkeleton?.Invoke(SkeletonLink);

            return base.OnClick(e);
        }

        protected override bool OnHover(HoverEvent e)
        {
            removeButton.FadeIn(200, Easing.OutQuint);

            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            removeButton.FadeOut(150, Easing.In);

            base.OnHoverLost(e);
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
                portButton.Text = port;
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
