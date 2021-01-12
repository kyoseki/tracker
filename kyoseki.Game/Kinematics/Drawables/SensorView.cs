using System;
using System.Numerics;
using kyoseki.Game.Serial;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;

namespace kyoseki.Game.Kinematics.Drawables
{
    public class SensorView : Drawable
    {
        private readonly Bindable<Quaternion> orientation = new Bindable<Quaternion>(Quaternion.Identity);

        private SensorLink link;

        public SensorLink Link
        {
            get => link;
            set
            {
                if (value == null)
                {
                    link = null;
                    orientation.UnbindBindings();

                    return;
                }

                link = value;
                orientation.BindTarget = link.CalibratedOrientation;
            }
        }

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

                DrawAxes(screenSpaceDrawQuad.Centre, Math.Min(screenSpaceDrawQuad.Width, screenSpaceDrawQuad.Height) / 2, Source.orientation.Value);
            }
        }
    }
}
