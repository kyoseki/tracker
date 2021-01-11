using System;
using osu.Framework.Allocation;

namespace kyoseki.Game.Serial
{
    public class SerialDependencies : DependencyContainer
    {
        protected ConnectionManager SerialConnections { get; }

        protected SkeletonLinkManager SkeletonLinks { get; }

        public SerialDependencies(IReadOnlyDependencyContainer parent)
            : base(parent)
        {
            Cache(SerialConnections = new ConnectionManager());
            Cache(SkeletonLinks = new SkeletonLinkManager());
        }
    }
}