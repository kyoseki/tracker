using System;
using System.Numerics;
using kyoseki.Game.MathUtils;
using kyoseki.Game.Overlays.Skeleton;
using osu.Framework.Bindables;

namespace kyoseki.Game.Serial
{
    public class SensorLink
    {
        public string BoneName { get; set; }

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

        public SensorLinkInfo Info => new SensorLinkInfo(BoneName, SensorId, MountOrientation);

        /// <summary>
        /// The final orientation of this sensor, taking both the <see cref="transform"/> and
        /// <see cref="calibrationOrientation"/> into account.
        /// </summary>
        public Bindable<Quaternion> CalibratedOrientation = new Bindable<Quaternion>();

        private const MountOrientation default_orientation = MountOrientation.ZUpYForward;

        private MountOrientation mountOrientation = default_orientation;

        /// <summary>
        /// Additional transform to apply improper rotations to the final orientation.
        /// By default, the Y and Z axes are swapped.
        /// </summary>
        private Matrix4x4 transform = SensorMountOrientations.Get(default_orientation);

        public MountOrientation MountOrientation
        {
            get => mountOrientation;
            set
            {
                mountOrientation = value;
                transform = SensorMountOrientations.Get(value);
            }
        }

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

        public void Calibrate() => calibrationOrientation = Quaternion.Inverse(lastOrientation);

        public void Update(Quaternion quat)
        {
            lastOrientation = quat;
            var relative = calibrationOrientation * quat;

            CalibratedOrientation.Value = relative.ApplyTransform(transform);
        }
    }

    public class SensorLinkInfo : IEquatable<SensorLinkInfo>
    {
        public readonly string BoneName;

        public readonly int SensorId;

        public readonly MountOrientation MountOrientation;

        public SensorLinkInfo(string boneName, int sensorId, MountOrientation mountOrientation)
        {
            BoneName = boneName;
            SensorId = sensorId;
            MountOrientation = mountOrientation;
        }

        public bool Equals(SensorLinkInfo other) =>
            BoneName == other?.BoneName &&
            SensorId == other?.SensorId &&
            MountOrientation == other.MountOrientation;
    }
}
