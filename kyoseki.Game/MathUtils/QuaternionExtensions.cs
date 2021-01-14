using System;
using System.Numerics;

namespace kyoseki.Game.MathUtils
{
    public static class QuaternionExtensions
    {
        // https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
        // TODO: a better function may exist
        public static (float x, float y, float z) ToEuler(this Quaternion q, bool degrees = true)
        {
            float sinrCosp = 2 * (q.W * q.X + q.Y * q.Z);
            float cosrCosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            float roll = MathF.Atan2(sinrCosp, cosrCosp);

            float sinp = 2 * (q.W * q.Y - q.Z * q.X);
            float pitch;
            if (MathF.Abs(sinp) >= 1)
                pitch = MathF.PI / 2 * (sinp < 0 ? -1 : 1);
            else
                pitch = MathF.Asin(sinp);

            float sinyCosp = 2 * (q.W * q.Z + q.X * q.Y);
            float cosyCosp = 1 - 2 * (q.Y * q.Y + q.Z + q.Z);
            float yaw = MathF.Atan2(sinyCosp, cosyCosp);

            if (degrees)
            {
                roll *= 180 / MathF.PI;
                pitch *= 180 / MathF.PI;
                yaw *= 180 / MathF.PI;
            }

            return (roll, pitch, yaw);
        }
    }
}
