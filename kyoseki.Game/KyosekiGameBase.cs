using kyoseki.Game.Input.Bindings;
using kyoseki.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osuTK;

namespace kyoseki.Game
{
    public class KyosekiGameBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.

        protected override Container<Drawable> Content { get; }

        protected KyosekiGameBase()
        {
            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1366, 768),
                Child = Content = new GlobalActionContainer
                {
                    RelativeSizeAxes = Axes.Both
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new DllResourceStore(typeof(KyosekiResources).Assembly));

            AddFont(Resources, @"Fonts/Manrope");
            AddFont(Resources, @"Fonts/Manrope-Bold");
            AddFont(Resources, @"Fonts/JetbrainsMono");
        }
    }
}
