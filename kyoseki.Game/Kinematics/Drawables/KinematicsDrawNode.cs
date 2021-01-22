using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Textures;
using osuTK;
using Vector3 = System.Numerics.Vector3; // feeling the pain yet?

namespace kyoseki.Game.Kinematics.Drawables
{
    /// <summary>
    /// An extension of <see cref="DrawNode"/> for drawing basic kinematics diagrams
    /// </summary>
    public class KinematicsDrawNode : DrawNode
    {
        protected KinematicsDrawNode(IDrawable source)
            : base(source)
        {
        }

        public const float BONE_NODE_SIZE = 1.4f;

        /// <summary>
        /// Draws a 3-axis diagram of a given rotation
        /// </summary>
        /// <param name="origin">The origin point of these axes (where they intersect)</param>
        /// <param name="length">The length of each axis</param>
        /// <param name="rotation">A quaternion representing the rotation the axes should show</param>
        /// <param name="vertexAction">Action to be performed on each vertex - used for textured sprites or batches</param>
        protected void DrawAxes(Vector2 origin, float length, System.Numerics.Quaternion rotation, Action<TexturedVertex2D> vertexAction = null)
        {
            var x = new Vector3(1, 0, 0);
            var y = new Vector3(0, 1, 0);
            var z = new Vector3(0, 0, 1);

            var xRot = Vector3.Transform(x, rotation) * length;
            var yRot = Vector3.Transform(y, rotation) * length;
            var zRot = Vector3.Transform(z, rotation) * length;

            // Y offsets are all inverted due to conflicting coord systems
            DrawLine(Texture.WhitePixel, origin, origin + new Vector2(xRot.X, -xRot.Y), 2, Colour4.Red, vertexAction);
            DrawLine(Texture.WhitePixel, origin, origin + new Vector2(yRot.X, -yRot.Y), 2, Colour4.Green, vertexAction);
            DrawLine(Texture.WhitePixel, origin, origin + new Vector2(zRot.X, -zRot.Y), 2, Colour4.Blue, vertexAction);
        }

        /// <summary>
        /// Draws a line
        /// </summary>
        protected void DrawLine(Texture texture, Vector2 p1, Vector2 p2, float width, ColourInfo colour, Action<TexturedVertex2D> vertexAction = null)
        {
            var angle = MathF.Atan2(p2.Y - p1.Y, p2.X - p1.X);

            var angle1 = angle + MathF.PI / 2;
            var angle2 = angle - MathF.PI / 2;

            var vec1 = new Vector2(MathF.Cos(angle1), MathF.Sin(angle1)) * width / 2;
            var vec2 = new Vector2(MathF.Cos(angle2), MathF.Sin(angle2)) * width / 2;

            var quad = new Quad(p1 + vec1, p1 + vec2, p2 + vec1, p2 + vec2);

            DrawQuad(texture, quad, colour, null, vertexAction);
        }

        /// <summary>
        /// Draws a square with a given size,
        /// centered at a given point
        /// </summary>
        protected void DrawPoint(Texture texture, Vector2 p, Vector2 size, ColourInfo colour, Action<TexturedVertex2D> vertexAction = null)
        {
            var pQuad = p - size / 2;

            DrawQuad(texture, new Quad(pQuad.X, pQuad.Y, size.X, size.Y), colour, null, vertexAction);
        }

        private void drawSingleBone(float scale, Quad drawQuad, Bone bone, Action<TexturedVertex2D> vertexAction = null)
        {
            var origin = drawQuad.Centre;

            var rootScaled = bone.Root2D * scale;
            var endScaled = bone.End2D * scale;

            var p1 = origin + rootScaled;
            var p2 = origin + endScaled;

            DrawLine(Texture.WhitePixel, p1, p2, 5, Colour4.Blue, vertexAction);

            var quadSize = new Vector2(BONE_NODE_SIZE) * scale;

            DrawPoint(Texture.WhitePixel, p1, quadSize, Colour4.Red, vertexAction);
            DrawPoint(Texture.WhitePixel, p2, quadSize, Colour4.Red, vertexAction);

            DrawAxes(p1, scale * 3, bone.WorldRotationNumerics);
        }

        /// <summary>
        /// Draws a bone - its root and end points, an axis diagram at the root, and a line connecting them
        /// </summary>
        /// <param name="scale">Scale factor for each bone's length</param>
        /// <param name="drawQuad">Draw quad of this Drawable</param>
        /// <param name="bone">Which bone to draw</param>
        /// <param name="vertexAction">Action to be performed on each vertex - used for textured sprites or batches</param>
        protected void DrawBone(float scale, Quad drawQuad, Bone bone, Action<TexturedVertex2D> vertexAction = null)
        {
            var bones = new List<Bone>();
            bone.Traverse(bones.Add);
            bones = bones.OrderBy(b => -b.EndPoint.Z).ThenBy(b => bones.IndexOf(b)).ToList();

            bones.ForEach(b => drawSingleBone(scale, drawQuad, b, vertexAction));
        }
    }
}
