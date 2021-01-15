using System;
using System.Collections.Generic;
using kyoseki.Game.Kinematics;

namespace kyoseki.Game.Serial
{
    public class SkeletonLink
    {
        public readonly Skeleton Skeleton = Skeleton.Default;

        private readonly List<SensorLink> sensors = new List<SensorLink>();

        public string Port { get; set; }

        public int ReceiverId { get; set; }

        public SkeletonLinkInfo Info => new SkeletonLinkInfo(Port, ReceiverId);

        public SensorLink Get(string boneName, bool allowNulls = false)
        {
            var existing = sensors.Find(s => s.BoneName == boneName);
            if (existing == null && !allowNulls)
                throw new InvalidOperationException($"A sensor was never linked to bone {boneName}.");

            return existing;
        }

        public SensorLink Get(int id, bool allowNulls = false)
        {
            var existing = sensors.Find(s => s.SensorId == id);
            if (existing == null && !allowNulls)
                throw new InvalidOperationException($"A sensor with ID {id} was never registered.");

            return existing;
        }

        public bool Register(int sensorId, string boneName, bool allowNulls = false)
        {
            var bone = Skeleton.GetBone(boneName);
            var link = Get(sensorId, allowNulls);
            if (link == null)
                return false;

            link.BoneName = boneName;

            link.CalibratedOrientation.UnbindEvents();
            link.CalibratedOrientation.ValueChanged += e =>
            {
                bone.WorldRotation = e.NewValue;
            };

            return true;
        }

        public void CalibrateAll() => sensors.ForEach(s => s.Calibrate());

        public void Remove(int sensorId)
        {
            var link = Get(sensorId);

            link.CalibratedOrientation.UnbindEvents();
            sensors.Remove(link);
        }

        public void Update(ReceiverMessage msg)
        {
            var targetLink = Get(msg.SensorId, true);

            if (targetLink == null)
            {
                sensors.Add(new SensorLink
                {
                    SensorId = msg.SensorId
                });

                return;
            }

            if (string.IsNullOrEmpty(targetLink.BoneName))
                return;

            targetLink.Update(msg.Quaternion);
        }
    }

    public class SkeletonLinkInfo : IEquatable<SkeletonLinkInfo>
    {
        public readonly string Port;

        public readonly int ReceiverId;

        public SkeletonLinkInfo(string port, int receiverId)
        {
            Port = port;
            ReceiverId = receiverId;
        }

        public bool Equals(SkeletonLinkInfo other) =>
            Port == other?.Port &&
            ReceiverId == other?.ReceiverId;
    }
}
