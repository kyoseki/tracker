using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Threading;

namespace kyoseki.Game.Serial
{
    public class SkeletonLinkManager : Component
    {
        [Resolved]
        private ConnectionManager serialConnections { get; set; }

        private readonly List<SkeletonSerialProcessor> ports = new List<SkeletonSerialProcessor>();

        // TODO: do better
        public readonly BindableList<int> ReceiverIds = new BindableList<int>();

        public new ScheduledDelegate Schedule(Action action) => base.Schedule(action);

        protected override void LoadComplete()
        {
            base.LoadComplete();

            serialConnections.PortsUpdated += handlePortsUpdated;
            serialConnections.MessageReceived += handleMessage;

            serialConnections.PortNames.ForEach(p => registerPort(p));
        }

        public SkeletonSerialProcessor GetPort(string port) => ports.Find(p => p.Port == port);

        public void Register(SkeletonLink link, string port)
        {
            lock (ports)
            {
                lock (link.Sensors)
                {
                    var oldPort = GetPort(link.Port);

                    oldPort?.Unregister(link);

                    var newPort = GetPort(port);

                    newPort.Register(link);
                    link.Port = port;
                    link.Sensors.RemoveAll(l => string.IsNullOrEmpty(l.BoneName));
                }
            }
        }

        private SkeletonSerialProcessor registerPort(string port)
        {
            lock (ports)
            {
                var newPort = new SkeletonSerialProcessor(this, port);
                ports.Add(newPort);
                return newPort;
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            serialConnections.PortsUpdated -= handlePortsUpdated;
            serialConnections.MessageReceived -= handleMessage;
        }

        private void handlePortsUpdated(string[] added, string[] removed)
        {
            added.ForEach(p => registerPort(p));
        }

        private void handleMessage(MessageInfo msg)
        {
            lock (ports)
            {
                var port = ports.Find(p => p.Port == msg.Port);
                port?.Update(msg);
            }
        }
    }

    public class SkeletonSerialProcessor
    {
        public readonly string Port;

        public readonly BindableList<int> ReceiverIds = new BindableList<int>();

        private readonly List<SkeletonLink> links = new List<SkeletonLink>();

        private readonly SkeletonLinkManager skeletonLinks;

        public SkeletonSerialProcessor(SkeletonLinkManager skeletonLinks, string port)
        {
            this.skeletonLinks = skeletonLinks;
            Port = port;
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

        public void Update(MessageInfo msg)
        {
            var split = msg.Content.Split(" ");

            if (int.TryParse(split[0], out int receiverId) &&
                int.TryParse(split[1], out int sensorId) &&
                float.TryParse(split[2], out float w) &&
                float.TryParse(split[3], out float x) &&
                float.TryParse(split[4], out float y) &&
                float.TryParse(split[5], out float z))
            {
                Quaternion quat = new Quaternion(x, y, z, w);

                skeletonLinks.Schedule(() =>
                {
                    if (!ReceiverIds.Contains(receiverId))
                        ReceiverIds.Add(receiverId);
                });

                var link = links.Find(l => l.Port == Port && l.ReceiverId == receiverId);
                link?.Update(new ReceiverMessage(sensorId, quat));
            }
        }
    }
}
