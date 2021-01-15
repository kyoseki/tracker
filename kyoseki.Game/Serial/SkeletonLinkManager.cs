using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace kyoseki.Game.Serial
{
    public class SkeletonLinkManager : Component
    {
        [Resolved]
        private ConnectionManager serialConnections { get; set; }

        private readonly List<SkeletonLink> links = new List<SkeletonLink>();

        // TODO: do better
        public readonly BindableList<int> ReceiverIds = new BindableList<int>();

        protected override void LoadComplete()
        {
            base.LoadComplete();

            serialConnections.MessageReceived += handleMessage;
        }

        public void Register(SkeletonLink link)
        {
            if (links.Any(l => l.Info == link.Info))
            {
                throw new InvalidOperationException("A link for this exact skeleton has already been registered.");
            }

            links.Add(link);
        }

        public void Unregister(SkeletonLink link) => links.Remove(link);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            serialConnections.MessageReceived -= handleMessage;
        }

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

                    Schedule(() =>
                    {
                        if (!ReceiverIds.Contains(receiverId))
                            ReceiverIds.Add(receiverId);
                    });

                    var link = links.Find(l => l.Port == msg.Port && l.ReceiverId == receiverId);
                    link?.Update(new ReceiverMessage(sensorId, quat));
                }
            }
        }
    }
}
