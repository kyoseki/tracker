﻿using System;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Events;

namespace kyoseki.Game.Kinematics.Drawables
{
    public class DrawableSkeleton : Drawable
    {
        public Skeleton Skeleton { get; set; } = Skeleton.Default;

        protected virtual float SkeletonDrawScale =>
            Math.Max(ScreenSpaceDrawQuad.Width, ScreenSpaceDrawQuad.Height) / 200;

        public Action<Bone> BoneClicked;

        protected override DrawNode CreateDrawNode() => new SkeletonDrawNode(this);

        protected override bool OnClick(ClickEvent e)
        {
            var relativePos = (e.ScreenSpaceMouseDownPosition - ScreenSpaceDrawQuad.Centre) / SkeletonDrawScale;
            var distances = Skeleton.Bones.Select(b => Vector2Extensions.Distance(b.Root2D, relativePos));
            var close = Skeleton.Bones.Zip(distances, (bone, distance) => (bone, distance))
                                .Where(b => b.distance < MathF.Sqrt(2) * KinematicsDrawNode.BONE_NODE_SIZE)
                                .OrderBy(b => b.distance);

            if (close.Any())
            {
                BoneClicked?.Invoke(close.First().bone);
                return true;
            }

            return base.OnClick(e);
        }

        private class SkeletonDrawNode : KinematicsDrawNode
        {
            protected new DrawableSkeleton Source => (DrawableSkeleton)base.Source;

            private Skeleton skeleton;
            private Quad drawQuad;

            private QuadBatch<TexturedVertex2D> vertexBatch;

            public SkeletonDrawNode(IDrawable source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                skeleton = Source.Skeleton;
                drawQuad = Source.ScreenSpaceDrawQuad;

                vertexBatch = new QuadBatch<TexturedVertex2D>(skeleton.BoneCount * 6, 1);
            }

            public override void Draw(Action<TexturedVertex2D> vertexAction)
            {
                base.Draw(vertexAction);

                skeleton = Source.Skeleton;

                DrawBone(Source.SkeletonDrawScale, drawQuad, skeleton.Root, vertexBatch.AddAction);
            }
        }
    }
}
