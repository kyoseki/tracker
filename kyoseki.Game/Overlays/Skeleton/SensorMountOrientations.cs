using System;
using System.Numerics;

namespace kyoseki.Game.Overlays.Skeleton
{
    public enum MountOrientation
    {
        ZUpYForward,
        YUpZForward
    }

    public static class SensorMountOrientations
    {
        private static readonly Matrix4x4 z_up_y_forward = new Matrix4x4(
            1, 0, 0, 0,
            0, 0, 1, 0,
            0, 1, 0, 0,
            0, 0, 0, 1
        );

        private static readonly Matrix4x4 y_up_z_forward = Matrix4x4.Identity;

        public static Matrix4x4 Get(MountOrientation orientation)
        {
            switch (orientation)
            {
                case MountOrientation.ZUpYForward:
                    return z_up_y_forward;

                case MountOrientation.YUpZForward:
                    return y_up_z_forward;

                default:
                    throw new ArgumentException("Provided mounting orientation was invalid");
            }
        }
    }
}
