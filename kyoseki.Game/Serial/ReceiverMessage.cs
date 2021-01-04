using System.Numerics;

namespace kyoseki.Game.Serial
{
    public class ReceiverMessage
    {
        public readonly int SensorId;

        public readonly Quaternion Quaternion;

        public ReceiverMessage(int sensorId, Quaternion quaternion)
        {
            SensorId = sensorId;
            Quaternion = quaternion;
        }
    }
}
