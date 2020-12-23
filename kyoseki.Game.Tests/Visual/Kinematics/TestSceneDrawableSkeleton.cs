using System;
using kyoseki.Game.Kinematics;
using kyoseki.Game.Kinematics.Drawables;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osuTK;

namespace kyoseki.Game.Tests.Visual.Kinematics
{
    public class TestSceneDrawableSkeleton : TestScene
    {
        public TestSceneDrawableSkeleton()
        {
            var skeleton = Skeleton.DEFAULT_SKELETON;

            var hips = skeleton.GetBone("Hips");
            hips.Rotation = Quaternion.FromEulerAngles(0, (float)Math.PI / 4, 0);
            Add(new DrawableSkeleton(Skeleton.DEFAULT_SKELETON)
            {
                RelativeSizeAxes = Axes.Both,
                Origin = Anchor.TopLeft,
                Anchor = Anchor.TopLeft
            });
        }
    }
}
