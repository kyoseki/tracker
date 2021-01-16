using System;
using System.Numerics;
using kyoseki.Game.Overlays.Skeleton;
using kyoseki.Game.Serial;
using osu.Framework.Graphics;
using osu.Framework.Testing;

namespace kyoseki.Game.Tests.Visual.Overlays
{
    public class TestSceneSensorLinkView : TestScene
    {
        public TestSceneSensorLinkView()
        {
            SensorLink link = new SensorLink
            {
                BoneName = "Test",
                SensorId = 255,
                MountOrientation = MountOrientation.YUpZForward
            };

            Add(new SensorLinkView(link)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            });

            var x = 0f;
            var y = 0f;
            var z = 0f;

            void updateRotation()
            {
                link.Update(Quaternion.CreateFromYawPitchRoll(y, x, z));
            }

            AddSliderStep("X", 0, MathF.PI * 2, 0, newX =>
            {
                x = newX;
                updateRotation();
            });

            AddSliderStep("Y", 0, MathF.PI * 2, 0, newY =>
            {
                y = newY;
                updateRotation();
            });

            AddSliderStep("Z", 0, MathF.PI * 2, 0, newZ =>
            {
                z = newZ;
                updateRotation();
            });
        }
    }
}
