using System.Linq;
using kyoseki.Game.Overlays.SerialMonitor;
using kyoseki.Game.Overlays.Skeleton;
using kyoseki.Game.Screens.Main;
using kyoseki.Game.Serial;
using kyoseki.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;

namespace kyoseki.Game.Screens
{
    public class MainScreen : Screen
    {
        private FillFlowContainer<SkeletonCard> skeletonFlow;

        private SkeletonLinkManager skeletonLinks;
        private SerialMonitorOverlay serial;
        private SkeletonOverlay skeletons;

        [BackgroundDependencyLoader]
        private void load(SkeletonLinkManager skeletonLinks, SerialMonitorOverlay serial, SkeletonOverlay skeletons)
        {
            this.skeletonLinks = skeletonLinks;
            this.serial = serial;
            this.skeletons = skeletons;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = KyosekiColors.Background.Darken(0.9f)
                },
                new KyosekiScrollContainer(Direction.Horizontal)
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Top = Toolbar.HEIGHT, Horizontal = 15 },
                    Child = skeletonFlow = new FillFlowContainer<SkeletonCard>
                    {
                        AutoSizeAxes = Axes.X,
                        RelativeSizeAxes = Axes.Y
                    }
                },
                new Toolbar
                {
                    RelativeSizeAxes = Axes.X,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    OpenSerial = serial.Show,
                    AddSkeleton = () =>
                    {
                        skeletons.SetLink(new SkeletonLink(), true);
                        skeletons.Show();
                    }
                }
            };

            skeletonLinks.LinkCreated += handleLinkCreated;
            skeletonLinks.LinkRemoved += handleLinkRemoved;

            foreach (var link in skeletonLinks.SkeletonLinks)
            {
                handleLinkCreated(link);
            }
        }

        private void handleLinkCreated(SkeletonLink link) => Schedule(() =>
        {
            skeletonFlow.Add(new SkeletonCard
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                SkeletonLink = link,
                OpenPort = serial.Open,
                OpenSkeleton = l =>
                {
                    skeletons.SetLink(l);
                    skeletons.Show();
                },
                RemoveSkeleton = skeletonLinks.Unregister
            });
        });

        private void handleLinkRemoved(SkeletonLink link)
        {
            var card = skeletonFlow.Children.FirstOrDefault(c => c.SkeletonLink.Info.Equals(link.Info));

            Schedule(() =>
            {
                card?.FadeOut(200, Easing.InQuint).Expire();
            });
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            skeletonLinks.LinkCreated -= handleLinkCreated;
            skeletonLinks.LinkRemoved -= handleLinkRemoved;
        }
    }
}
