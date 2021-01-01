using System;
using System.Numerics;
using kyoseki.Game.Serial;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;

namespace kyoseki.Game.Kinematics.Drawables
{
    public class SensorView : Drawable
    {
        private Bindable<Quaternion> orientation = new Bindable<Quaternion>(Quaternion.Identity);

        private string port;

        private int? bodyId;

        private int? sensorId;

        [Resolved]
        private ConnectionManager serialConnections { get; set; }

        protected override DrawNode CreateDrawNode() => new SensorViewDrawNode(this);

        protected override void LoadComplete()
        {
            base.LoadComplete();

            serialConnections.MessageReceived += handleMessage;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            serialConnections.MessageReceived -= handleMessage;
        }

        private void handleMessage(string port, string msg)
        {
            if (port == this.port || this.port == null)
            {
                var segments = msg.Split(" ");

                if (segments.Length == 6)
                {
                    var bodyId = int.Parse(segments[0]);
                    var sensorId = int.Parse(segments[1]);

                    this.bodyId ??= bodyId;
                    this.sensorId ??= sensorId;

                    if (bodyId == this.bodyId && sensorId == this.sensorId)
                    {
                        orientation.Value = new Quaternion(
                            float.Parse(segments[3]),
                            float.Parse(segments[4]),
                            float.Parse(segments[5]),
                            float.Parse(segments[2])
                            );
                    }

                    this.port ??= port;
                }
            }
        }

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
