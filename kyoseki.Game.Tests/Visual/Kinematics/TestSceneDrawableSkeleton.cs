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
            Add(new DrawableSkeleton(Skeleton.DEFAULT_SKELETON)
            {
                RelativeSizeAxes = Axes.Both,
                Origin = Anchor.TopLeft,
                Anchor = Anchor.TopLeft
            });

            foreach (var bone in skeleton.Bones)
            {
                var x = 0f;
                var y = 0f;
                var z = 0f;

                Action updateRotation = () =>
                {
                    bone.Rotation = System.Numerics.Quaternion.CreateFromYawPitchRoll(y, x, z);
                };

                AddSliderStep($"{bone.Name} X", 0, (float)Math.PI * 2, 0, value =>
                {
                    x = value;
                    updateRotation();
                });

                AddSliderStep($"{bone.Name} Y", 0, (float)Math.PI * 2, 0, value =>
                {
                    y = value;
                    updateRotation();
                });

                AddSliderStep($"{bone.Name} Z", 0, (float)Math.PI * 2, 0, value =>
                {
                    z = value;
                    updateRotation();
                });
            }
        }
    }
}
