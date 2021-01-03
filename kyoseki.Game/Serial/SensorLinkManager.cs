using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace kyoseki.Game.Serial
{
    public class SensorLinkManager : Component
    {
        [Resolved]
        private ConnectionManager serialConnections { get; set; }

        private readonly List<SensorLink> links = new List<SensorLink>();

        protected override void LoadComplete()
        {
            base.LoadComplete();

            serialConnections.MessageReceived += handleMessage;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            serialConnections.MessageReceived -= handleMessage;
        }

        public void Register(SensorLink link)
        {
            if (links.Any(l => l.Represents(link)))
            {
                throw new InvalidOperationException("A link for this exact sensor has already been registered.");
            }
            links.Add(link);
        }

        public void Unregister(SensorLink link) => links.Remove(link);

        private void handleMessage(MessageInfo msg)
        {
            var split = msg.Content.Split(" ");

            if (split.Length == 6)
            {
                if (int.TryParse(split[0], out int receiverId) &&
                    int.TryParse(split[1], out int sensorId) &&
                    float.TryParse(split[2], out float w) &&
                    float.TryParse(split[3], out float x) &&
                    float.TryParse(split[4], out float y) &&
                    float.TryParse(split[5], out float z))
                {
                    Quaternion quat = new Quaternion(x, y, z, w);

                    var link = links.Find(l => l.Represents(msg.Port, receiverId, sensorId));
                    link?.Update(quat);
                }
            }
        }
    }
}
