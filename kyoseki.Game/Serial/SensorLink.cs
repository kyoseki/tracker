using System.Numerics;
using osu.Framework.Bindables;

namespace kyoseki.Game.Serial
{
    public class SensorLink
    {
        public readonly string Port;

        public readonly int ReceiverId;

        public readonly int SensorId;

        private Quaternion lastOrientation;

        private Quaternion calibrationOrientation = Quaternion.Identity;

        public Bindable<Quaternion> CalibratedOrientation = new Bindable<Quaternion>();

        public SensorLink(string port, int receiverId, int sensorId)
        {
            Port = port;
            ReceiverId = receiverId;
            SensorId = sensorId;
        }

        public void Calibrate() => calibrationOrientation = Quaternion.Inverse(lastOrientation);

        public bool Represents(string port, int receiverId, int sensorId) =>
            Port == port &&
            ReceiverId == receiverId &&
            SensorId == sensorId;

        public bool Represents(SensorLink link) =>
            Represents(link.Port, link.ReceiverId, link.SensorId);

        public void Update(Quaternion quat)
        {
            lastOrientation = quat;
            CalibratedOrientation.Value = calibrationOrientation * quat;
        }
    }
}
