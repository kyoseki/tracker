using System;
using System.Collections.Generic;
using System.Linq;
using kyoseki.Game.Kinematics;

namespace kyoseki.Game.Serial
{
    public class SkeletonLink
    {
        public readonly Skeleton Skeleton = Skeleton.Default;

        public readonly List<SensorLink> Sensors = new List<SensorLink>();

        public event Action SensorsUpdated;

        public string Port { get; set; }

        public int ReceiverId { get; set; }

        public SkeletonLinkInfo Info
        {
            get
            {
                lock (Sensors)
                    return new SkeletonLinkInfo(Port, ReceiverId, Sensors.Select(s => s.Info).ToArray());
            }
            set
            {
                Port = value.Port;
                ReceiverId = value.ReceiverId;

                lock (Sensors)
                {
                    foreach (var sensorInfo in value.Sensors)
                    {
                        Sensors.Add(new SensorLink
                        {
                            BoneName = sensorInfo.BoneName,
                            SensorId = sensorInfo.SensorId,
                            MountOrientation = sensorInfo.MountOrientation
                        });

                        Register(sensorInfo.SensorId, sensorInfo.BoneName);
                    }
                }
            }
        }

        public SensorLink Get(string boneName, bool allowNulls = false)
        {
            lock (Sensors)
            {
                var existing = Sensors.Find(s => s.BoneName == boneName);
                if (existing == null && !allowNulls)
                    throw new InvalidOperationException($"A sensor was never linked to bone {boneName}.");

                return existing;
            }
        }

        public SensorLink Get(int id, bool allowNulls = false)
        {
            lock (Sensors)
            {
                var existing = Sensors.Find(s => s.SensorId == id);
                if (existing == null && !allowNulls)
                    throw new InvalidOperationException($"A sensor with ID {id} was never registered.");

                return existing;
            }
        }

        public bool Register(int sensorId, string boneName, bool allowNulls = false)
        {
            var bone = Skeleton.GetBone(boneName);
            var link = Get(sensorId, allowNulls);
            if (link == null)
                return false;

            var boneLink = Get(boneName, true);

            if (boneLink != null)
            {
                boneLink.CalibratedOrientation.UnbindEvents();
                boneLink.BoneName = null;
            }

            link.BoneName = boneName;

            link.CalibratedOrientation.UnbindEvents();
            link.CalibratedOrientation.ValueChanged += e =>
            {
                bone.WorldRotation = e.NewValue;
            };

            return true;
        }

        public void CalibrateAll()
        {
            lock (Sensors)
            {
                Sensors.ForEach(s => s.Calibrate());
            }
        }

        public void Remove(int sensorId)
        {
            var link = Get(sensorId);

            link.CalibratedOrientation.UnbindEvents();

            lock (Sensors)
                Sensors.Remove(link);
        }

        public void Update(ReceiverMessage msg)
        {
            var targetLink = Get(msg.SensorId, true);

            if (targetLink == null)
            {
                lock (Sensors)
                {
                    Sensors.Add(new SensorLink
                    {
                        SensorId = msg.SensorId
                    });

                    SensorsUpdated?.Invoke();
                }

                return;
            }

            targetLink.Update(msg.Quaternion);
        }
    }

    public class SkeletonLinkInfo : IEquatable<SkeletonLinkInfo>
    {
        public readonly string Port;

        public readonly int ReceiverId;

        public readonly SensorLinkInfo[] Sensors;

        public SkeletonLinkInfo(string port, int receiverId, SensorLinkInfo[] sensors)
        {
            Port = port;
            ReceiverId = receiverId;
            Sensors = sensors;
        }

        public bool Equals(SkeletonLinkInfo other) =>
            Port == other?.Port &&
            ReceiverId == other?.ReceiverId &&
            Sensors.SequenceEqual(other.Sensors);
    }
}
