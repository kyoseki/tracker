using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;

namespace kyoseki.Game.Kinematics.Drawables
{
    public class DrawableSkeleton : Drawable
    {
        public readonly Skeleton Skeleton;

        public DrawableSkeleton(Skeleton skeleton)
        {
            Skeleton = skeleton;
        }

        protected override DrawNode CreateDrawNode() => new SkeletonDrawNode(this);

        private class SkeletonDrawNode : KinematicsDrawNode
        {
            protected new DrawableSkeleton Source => (DrawableSkeleton)base.Source;

            private Skeleton skeleton;
            private Quad drawQuad;

            public SkeletonDrawNode(DrawableSkeleton source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                skeleton = Source.Skeleton;
                drawQuad = Source.ScreenSpaceDrawQuad;
            }

            public override void Draw(Action<TexturedVertex2D> vertexAction)
            {
                base.Draw(vertexAction);

                DrawBone(drawQuad, skeleton.Root);
            }
        }
    }
}
