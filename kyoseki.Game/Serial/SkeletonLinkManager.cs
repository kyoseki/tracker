using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using kyoseki.Game.Configuration;
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

        [Resolved(CanBeNull = true)]
        private KyosekiConfigManager config { get; set; }

        private readonly List<SkeletonSerialProcessor> ports = new List<SkeletonSerialProcessor>();

        public new ScheduledDelegate Schedule(Action action) => base.Schedule(action);

        public IEnumerable<SkeletonLink> SkeletonLinks
        {
            get
            {
                var result = new List<SkeletonLink>();

                foreach (var port in ports)
                {
                    result.AddRange(port.Links);
                }

                return result;
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            serialConnections.PortsUpdated += handlePortsUpdated;
            serialConnections.MessageReceived += handleMessage;

            serialConnections.PortNames.ForEach(p => registerPort(p));

            if (config == null)
                return;

            var savedPorts = config.Get<SkeletonSerialProcessorInfo[]>(KyosekiSetting.Skeletons);

            Load(savedPorts);

            Scheduler.AddDelayed(() =>
            {
                config.Set(KyosekiSetting.Skeletons, ports.Select(p => p.Info).Where(p => p.Links.Length > 0).ToArray());
                config.Save();
            }, 10000, true);
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

        public void Load(IEnumerable<SkeletonSerialProcessorInfo> infos)
        {
            foreach (var info in infos)
            {
                var port = GetPort(info.Port);

                if (port != null)
                {
                    port.Info = info;
                }
                else
                {
                    var newPort = registerPort(info.Port);

                    newPort.Info = info;
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

        public readonly List<SkeletonLink> Links = new List<SkeletonLink>();

        private readonly SkeletonLinkManager skeletonLinks;

        public SkeletonSerialProcessorInfo Info
        {
            get => new SkeletonSerialProcessorInfo(Port, Links.Select(l => l.Info).ToArray());
            set
            {
                if (value.Port != Port)
                    return;

                Links.AddRange(value.Links.Select(l => new SkeletonLink
                {
                    Info = l
                }));
            }
        }

        public SkeletonSerialProcessor(SkeletonLinkManager skeletonLinks, string port)
        {
            this.skeletonLinks = skeletonLinks;
            Port = port;
        }

        public void Register(SkeletonLink link)
        {
            if (Links.Any(l => l.Info == link.Info))
            {
                throw new InvalidOperationException("A link for this exact skeleton has already been registered.");
            }

            Links.Add(link);
        }

        public void Unregister(SkeletonLink link) => Links.Remove(link);

        private float parseInt(int n)
        {
            int sign = n >> 7;
            int val = n & 0x7F;

            int multiplier = sign == 1 ? -1 : 1;

            return val / 100f * multiplier;
        }

        public void Update(MessageInfo msg)
        {
            var split = msg.Content.Split(" ");

            if (split.Length != 6)
                return;

            if (int.TryParse(split[0], out int receiverId) &&
                int.TryParse(split[1], out int sensorId) &&
                int.TryParse(split[2], out int w) &&
                int.TryParse(split[3], out int x) &&
                int.TryParse(split[4], out int y) &&
                int.TryParse(split[5], out int z))
            {
                Quaternion quat = new Quaternion(parseInt(x), parseInt(y), parseInt(z), parseInt(w));

                skeletonLinks.Schedule(() =>
                {
                    if (!ReceiverIds.Contains(receiverId))
                        ReceiverIds.Add(receiverId);
                });

                var link = Links.Find(l => l.Port == Port && l.ReceiverId == receiverId);
                link?.Update(new ReceiverMessage(sensorId, quat));
            }
        }
    }

    public class SkeletonSerialProcessorInfo : IEquatable<SkeletonSerialProcessorInfo>
    {
        public readonly string Port;

        public readonly SkeletonLinkInfo[] Links;

        public SkeletonSerialProcessorInfo(string port, SkeletonLinkInfo[] links)
        {
            Port = port;
            Links = links;
        }

        public bool Equals(SkeletonSerialProcessorInfo other) =>
            Port == other?.Port &&
            Links.SequenceEqual(other?.Links);
    }
}
