﻿using System;
using System.Collections.Generic;
using kyoseki.Game.Kinematics;

namespace kyoseki.Game.Serial
{
    public class SkeletonLink
    {
        public readonly Skeleton Skeleton = Skeleton.DEFAULT_SKELETON;

        private readonly List<SensorLink> sensors = new List<SensorLink>();

        public string Port { get; set; }

        public int ReceiverId { get; set; }

        public SkeletonLinkInfo Info =>
            new SkeletonLinkInfo { Port = Port, ReceiverId = ReceiverId };

        public SkeletonLink()
        {
        }

        public SkeletonLink(string port, int receiverId)
        {
            Port = port;
            ReceiverId = receiverId;
        }

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

        public SensorLink Register(string boneName, int sensorId)
        {
            var bone = Skeleton.GetBone(boneName);
            var link = new SensorLink(boneName, sensorId);

            sensors.Add(link);
            bone.Rotation.BindTo(link.CalibratedOrientation);

            return link;
        }

        public void CalibrateAll() => sensors.ForEach(s => s.Calibrate());

        public void UpdateLink(string boneName, int sensorId) =>
            Get(boneName).SensorId = sensorId;

        public void Unregister(string boneName, int sensorId)
        {
            var bone = Skeleton.GetBone(boneName);
            var link = Get(boneName);

            sensors.Remove(link);
            bone.Rotation.UnbindBindings();
        }

        public bool Represents(string port, int receiverId) =>
            Port == port &&
            ReceiverId == ReceiverId;

        public void Update(ReceiverMessage msg)
        {
            var targetLink = Get(msg.SensorId, true);
            if (targetLink == null)
                return;

            targetLink.Update(msg.Quaternion);
        }
    }

    public class SkeletonLinkInfo : IEquatable<SkeletonLinkInfo>
    {
        public string Port { get; set; }

        public int ReceiverId { get; set; }

        public bool Equals(SkeletonLinkInfo other) =>
            Port == other.Port &&
            ReceiverId == other.ReceiverId;
    }
}
