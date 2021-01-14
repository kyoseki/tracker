using kyoseki.Game.Serial;
using osu.Framework.Allocation;
using osu.Framework.Testing;

namespace kyoseki.Game.Tests.Visual
{
    [ExcludeFromDynamicCompile]
    public abstract class KyosekiTestScene : TestScene
    {
        private DependencyContainer dependencies;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new SerialDependencies(base.CreateChildDependencies(parent));

        protected override void LoadComplete()
        {
            base.LoadComplete();

            Add(dependencies.Get<ConnectionManager>());
            Add(dependencies.Get<SkeletonLinkManager>());
        }
    }
}
