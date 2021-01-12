using System;
using System.Numerics;
using osu.Framework.Bindables;

namespace kyoseki.Game.Serial
{
    public class SensorLink
    {
        public readonly string BoneName;

        private int sensorId;

        public int SensorId
        {
            get => sensorId;
            set
            {
                calibrationOrientation = Quaternion.Identity;
                sensorId = value;
            }
        }

        public SensorLinkInfo Info => new SensorLinkInfo(BoneName, SensorId);

        /// <summary>
        /// The final orientation of this sensor, taking both the <see cref="Transform"/> and
        /// <see cref="calibrationOrientation"/> into account.
        /// </summary>
        public Bindable<Quaternion> CalibratedOrientation = new Bindable<Quaternion>();

        /// <summary>
        /// Additional transform to apply improper rotations to the final orientation.
        /// By default, the Y and Z axes are swapped.
        /// </summary>
        public Matrix4x4 Transform = new Matrix4x4(
            1, 0, 0, 0,
            0, 0, 1, 0,
            0, 1, 0, 0,
            0, 0, 0, 1
        );

        /// <summary>
        /// Represents the last raw rotation received.
        /// </summary>
        private Quaternion lastOrientation;

        /// <summary>
        /// Represents the inverse of the rotation the sensor was at when
        /// <see cref="Calibrate"/> was run.
        /// Multiplying this orientation by the <see cref="lastOrientation"/> will produce rotations
        /// relative to the one captured at calibration (i.e. the inverse of this orientation).
        /// </summary>
        private Quaternion calibrationOrientation = Quaternion.Identity;

        public SensorLink(string boneName, int sensorId)
        {
            BoneName = boneName;
            SensorId = sensorId;
        }

        public void Calibrate() => calibrationOrientation = Quaternion.Inverse(lastOrientation);

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

    public class SensorLinkInfo : IEquatable<SensorLinkInfo>
    {
        public readonly string BoneName;

        public readonly int SensorId;

        public SensorLinkInfo(string boneName, int sensorId)
        {
            BoneName = boneName;
            SensorId = sensorId;
        }

        public bool Equals(SensorLinkInfo other) =>
            BoneName == other?.BoneName &&
            SensorId == other?.SensorId;
    }
}
