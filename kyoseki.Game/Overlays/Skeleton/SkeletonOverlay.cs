using kyoseki.Game.Serial;
using kyoseki.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace kyoseki.Game.Overlays.Skeleton
{
    public class SkeletonOverlay : SlideInOverlay
    {
        private SkeletonEditor editor;

        [BackgroundDependencyLoader]
        private void load(SkeletonLinkManager skeletonLinks)
        {
            Children = new Drawable[]
            {
                new Box
                {
                    Colour = KyosekiColors.BACKGROUND.Darken(0.2f).Opacity(0.5f),
                    RelativeSizeAxes = Axes.Both
                },
                editor = new SkeletonEditor
                {
                    RelativeSizeAxes = Axes.Both
                }
            };

            skeletonLinks.Register(editor.Link);
        }
    }
}
