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
            CalibratedOrientation.Value = calibrationOrientation * quat;
        }
    }
}
