using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using kyoseki.Game.Overlays.SerialMonitor;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Threading;

namespace kyoseki.Game.Serial
{
    public class ConnectionManager : Component
    {
        private readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public List<string> PortNames = new List<string>();

        private List<ISerialPort> ports = new List<ISerialPort>();

        public Bindable<ConnectionState> State = new Bindable<ConnectionState>(ConnectionState.Resetting);

        public event Action<MessageInfo> MessageReceived;

        public event Action<string[], string[]> PortsUpdated;

        private Scheduler scheduler;

        public ConnectionManager()
        {
            var thread = new Thread(run)
            {
                Name = "Serial.ConnectionManager",
                IsBackground = true
            };

            thread.Start();
        }

        private void lookForPorts()
        {
            bool newPorts = !SerialPort.GetPortNames().All(p => PortNames.Contains(p));

            if (newPorts)
            {
                Logger.Log("New serial ports detected", LoggingTarget.Network);
                State.Value = ConnectionState.Resetting;
            }
        }

        private void run()
        {
            scheduler = new Scheduler();
            scheduler.AddDelayed(lookForPorts, 2500, true);

            while (!cancellationToken.IsCancellationRequested)
            {
                switch (State.Value)
                {
                    case ConnectionState.Ready:
                        var toQuery = ports.Where(p => p.State == SerialPortState.Open);
                        foreach (var port in toQuery)
                        {
                            try
                            {
                                while (true)
                                {
                                    var message = port.ReadLine().Replace(port.NewLineRead, string.Empty);

                                    MessageReceived?.Invoke(new MessageInfo
                                    {
                                        Port = port.Name,
                                        Content = message,
                                        Direction = MessageDirection.Incoming,
                                        Timestamp = DateTime.Now
                                    });
                                }
                            }
                            catch (TimeoutException) { }
                            catch (OperationCanceledException) { }
                            catch (UnauthorizedAccessException) { }
                            catch (InvalidOperationException) { }
                        }
                        break;
                    case ConnectionState.Resetting:
                        var newPorts = SerialPort.GetPortNames();

                        // Ports that were physically connected
                        var added = newPorts.Where(p => !PortNames.Contains(p)).ToArray();
                        // Ports that were physically disconnected
                        var removed = PortNames.Where(p => !newPorts.Contains(p)).ToArray();

                        // Ports that were physically connected AND
                        // have never been connected before since the program started
                        var toCreate = added.Where(p => !ports.Any(po => po.Name == p));

                        lock (PortNames)
                        {
                            PortNames.Clear();
                            PortNames.AddRange(newPorts);

                            PortsUpdated?.Invoke(added, removed);
                        }

                        foreach (var port in toCreate)
                        {
                            SystemSerialPort s = new SystemSerialPort(port, 115200)
                            {
                                NewLineRead = "\r\n",
                                NewLineWrite = "\n",
                                ReadTimeout = 1,
                                ConnectionLost = () => State.Value = ConnectionState.Resetting
                        };

                            ports.Add(s);
                        }

                        foreach (var port in ports)
                        {
                            if (port.State == SerialPortState.Closed ||
                                (port.State == SerialPortState.Disconnected && added.Contains(port.Name)))
                                port.Open();
                        }

                        State.Value = ConnectionState.Ready;
                        break;
                }

                scheduler.Update();
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            cancellationToken.Cancel();

            ports.ForEach(p => p.Dispose());
        }
    }

    public class MessageInfo
    {
        public string Port { get; set; }

        public string Content { get; set; }

        public MessageDirection Direction { get; set; }

        public DateTime Timestamp { get; set; }

        public float ChannelYPosition { get; set; }
    }

    public enum ConnectionState
    {
        Ready,
        Resetting
    }
}
