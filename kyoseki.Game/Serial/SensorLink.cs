using System;
using System.Numerics;
using osu.Framework.Bindables;

namespace kyoseki.Game.Serial
{
    public class SensorLink
    {
        public readonly SensorLinkInfo Info;

        private Quaternion lastOrientation;

        private Quaternion calibrationOrientation = Quaternion.Identity;

        public Bindable<Quaternion> CalibratedOrientation = new Bindable<Quaternion>();

        public Matrix4x4 Transform = new Matrix4x4(
                1, 0, 0, 0,
                0, 0, 1, 0,
                0, 1, 0, 0,
                0, 0, 0, 1
            );

        public SensorLink(string port, int receiverId, int sensorId)
        {
            Info = new SensorLinkInfo
            {
                Port = port,
                ReceiverId = receiverId,
                SensorId = sensorId
            };
        }

        public void Calibrate() => calibrationOrientation = Quaternion.Inverse(lastOrientation);

        public bool Represents(string port, int receiverId, int sensorId) =>
            Info.Port == port &&
            Info.ReceiverId == receiverId &&
            Info.SensorId == sensorId;

        public bool Represents(SensorLink link) =>
            Represents(link.Info.Port, link.Info.ReceiverId, link.Info.SensorId);

        public void Update(Quaternion quat)
        {
            lastOrientation = quat;
            var relative = calibrationOrientation * quat;

            // https://stackoverflow.com/questions/1274936/flipping-a-quaternion-from-right-to-left-handed-coordinates 
            bool result = Matrix4x4.Invert(Transform, out Matrix4x4 tInverted);
            if (!result)
                throw new Exception("Failed to invert SensorLink transform matrix");

            Matrix4x4 mFinal = Transform * Matrix4x4.CreateFromQuaternion(relative) * tInverted;

            CalibratedOrientation.Value = Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(mFinal));
        }
    }
}
