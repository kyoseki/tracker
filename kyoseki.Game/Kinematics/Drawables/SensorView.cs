using System;
using kyoseki.Game.Serial;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;

namespace kyoseki.Game.Kinematics.Drawables
{
    public class SensorView : Drawable
    {
        public SensorLink Link { get; set; }

        protected override DrawNode CreateDrawNode() => new SensorViewDrawNode(this);

        private class SensorViewDrawNode : KinematicsDrawNode
        {
            protected new SensorView Source => (SensorView)base.Source;

            private Quad screenSpaceDrawQuad;

            public SensorViewDrawNode(SensorView source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                screenSpaceDrawQuad = Source.ScreenSpaceDrawQuad;
            }

            public override void Draw(Action<TexturedVertex2D> vertexAction)
            {
                base.Draw(vertexAction);

                if (Source.Link != null)
                    DrawAxes(screenSpaceDrawQuad.Centre, Math.Min(screenSpaceDrawQuad.Width, screenSpaceDrawQuad.Height) / 2, Source.Link.CalibratedOrientation.Value);
            }
        }
    }
}
