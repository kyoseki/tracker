using kyoseki.Game.Serial;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace kyoseki.Game.Overlays.Skeleton
{
    public class SkeletonOverlay : SlideInOverlay
    {
        protected override string Title => "Skeleton Editor";

        private SkeletonEditor editor;

        [BackgroundDependencyLoader]
        private void load(SkeletonLinkManager skeletonLinks)
        {
            Add(editor = new SkeletonEditor
            {
                RelativeSizeAxes = Axes.Both
            });

            skeletonLinks.Register(editor.Link);
        }
    }
}
