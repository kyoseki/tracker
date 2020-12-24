using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace kyoseki.Game.Kinematics.Drawables
{
    /// <summary>
    /// An extension of <see cref="DrawNode"/> for drawing basic kinematics diagrams
    /// </summary>
    public class KinematicsDrawNode : DrawNode
    {
        public KinematicsDrawNode(IDrawable source)
            : base(source)
        {
        }

        /// <summary>
        /// Draws a 3-axis diagram of a given rotation
        /// </summary>
        /// <param name="origin">The origin point of these axes (where they intersect)</param>
        /// <param name="length">The length of each axis</param>
        /// <param name="rotation">A quaternion representing the rotation the axes should show</param>
        public void DrawAxes(Vector2 origin, float length, Quaternion rotation)
        {
            var x = new Vector3(1, 0, 0);
            var y = new Vector3(0, -1, 0);
            var z = new Vector3(0, 0, 1);

            var xRot = Vector3.Transform(x, rotation) * length;
            var yRot = Vector3.Transform(y, rotation) * length;
            var zRot = Vector3.Transform(z, rotation) * length;

            DrawLine(Texture.WhitePixel, origin, origin + new Vector2(xRot.X, xRot.Y), 2, Colour4.Red);
            DrawLine(Texture.WhitePixel, origin, origin + new Vector2(yRot.X, yRot.Y), 2, Colour4.Green);
            DrawLine(Texture.WhitePixel, origin, origin + new Vector2(zRot.X, zRot.Y), 2, Colour4.Blue);
        }

        /// <summary>
        /// Draws a line
        /// </summary>
        public void DrawLine(Texture texture, Vector2 p1, Vector2 p2, float width, ColourInfo colour)
        {
            var angle = Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);

            var angle1 = angle + Math.PI / 2;
            var angle2 = angle - Math.PI / 2;

            var vec1 = new Vector2((float)Math.Cos(angle1), (float)Math.Sin(angle1)) * width / 2;
            var vec2 = new Vector2((float)Math.Cos(angle2), (float)Math.Sin(angle2)) * width / 2;

            var quad = new Quad(p1 + vec1, p1 + vec2, p2 + vec1, p2 + vec2);

            DrawQuad(texture, quad, colour);
        }

        /// <summary>
        /// Draws a square with a given size,
        /// centered at a given point
        /// </summary>
        public void DrawPoint(Texture texture, Vector2 p, Vector2 size, ColourInfo colour)
        {
            var pQuad = p - size / 2;

            DrawQuad(texture, new Quad(pQuad.X, pQuad.Y, size.X, size.Y), colour);
        }

        private void drawSingleBone(Quad drawQuad, Bone bone)
        {
            var scale = drawQuad.Width / 240;
            var origin = drawQuad.Centre;

            var rootScaled = bone.Root2D * scale;
            var endScaled = bone.End2D * scale;

            var p1 = origin + rootScaled;
            var p2 = origin + endScaled;

            DrawLine(Texture.WhitePixel, p1, p2, 5, Colour4.Blue);

            var quadSize = new Vector2(7, 7);

            DrawPoint(Texture.WhitePixel, p1, quadSize, Colour4.Red);
            if (!bone.HasChildren)
                DrawPoint(Texture.WhitePixel, p2, quadSize, Colour4.Red);

            DrawAxes(p1, scale * 3, bone.Rotation);
        }

        /// <summary>
        /// Draws a bone - its root and end points, an axis diagram at the root, and a line connecting them
        /// </summary>
        /// <param name="drawQuad">Draw quad of this Drawable</param>
        /// <param name="bone">Which bone to draw</param>
        public void DrawBone(Quad drawQuad, Bone bone)
        {
            bone.Traverse(bone =>
            {
                drawSingleBone(drawQuad, bone);
            });
        }
    }
}
