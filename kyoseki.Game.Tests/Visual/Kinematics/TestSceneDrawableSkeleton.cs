using System;
using kyoseki.Game.Kinematics;
using kyoseki.Game.Kinematics.Drawables;
using osu.Framework.Graphics;
using osu.Framework.Testing;

namespace kyoseki.Game.Tests.Visual.Kinematics
{
    public class TestSceneDrawableSkeleton : TestScene
    {
        public TestSceneDrawableSkeleton()
        {
            var skeleton = Skeleton.DEFAULT_SKELETON;

            Add(new DrawableSkeleton(skeleton)
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

                void updateRotation()
                {
                    bone.Rotation.Value = System.Numerics.Quaternion.CreateFromYawPitchRoll(y, x, z);
                }

                AddSliderStep($"{bone.Name} X", 0, MathF.PI * 2, 0, value =>
                {
                    x = value;
                    updateRotation();
                });

                AddSliderStep($"{bone.Name} Y", 0, MathF.PI * 2, 0, value =>
                {
                    y = value;
                    updateRotation();
                });

                AddSliderStep($"{bone.Name} Z", 0, MathF.PI * 2, 0, value =>
                {
                    z = value;
                    updateRotation();
                });
            }
        }
    }
}
